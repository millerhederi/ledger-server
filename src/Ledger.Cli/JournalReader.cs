using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ledger.WebApi.Models;

namespace Ledger.Cli
{
    public class JournalReader
    {
        private static readonly Regex DescriptionLineRegex = new Regex(@"(?<date>\d{4}/\d{1,2}/\d{1,2}) (?:(?<status>\!|\*) )?(?<description>.*?)(?: +; (?<comment>.*)|$)");
        private static readonly Regex PostingLineRegex = new Regex(@"    (?<account>.*?)(?: {2,}\$ *(?<amount>-?[\d,]*\.?\d*)(?: *; +(?<comment>.*))?|$)");

        private readonly ConcurrentDictionary<string, AccountModel> _accounts = new ConcurrentDictionary<string, AccountModel>();

        public async Task<ICollection<TransactionModel>> ReadFromStreamAsync(Stream stream)
        {
            var transactionModels = new List<TransactionModel>();

            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (!Regex.Match(line, @"^\d{4}/\d{1,2}/\d{1,2}").Success)
                    {
                        // Header line, do nothing with it
                    }
                    else
                    {
                        transactionModels.Add(await GetTransactionModelAsync(line, reader));
                    }
                }
            }

            return transactionModels;
        }

        public async Task<ICollection<TransactionModel>> ReadFromFileAsync(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                return await ReadFromStreamAsync(stream);
            }
        }

        private async Task<TransactionModel> GetTransactionModelAsync(string descriptionLine, TextReader reader)
        {
            var descriptionLineMatch = DescriptionLineRegex.Match(descriptionLine);

            if (!descriptionLineMatch.Success)
            {
                throw new InvalidOperationException($"Unable to parse the description line: '{descriptionLine}'.");
            }

            if (!DateTime.TryParseExact(descriptionLineMatch.Groups["date"].Value, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                throw new ArgumentOutOfRangeException($"Unable to parse the date: '{descriptionLineMatch.Groups["date"].Value}'.");
            }

            var description = descriptionLineMatch.Groups["description"].Value;

            var transactionModel = new TransactionModel
            {
                PostedDate = date,
                Description = description,
            };

            PostingModel emptyAmountPostingModel = null;
            var transactionBalance = 0m;

            string line;
            while (!string.IsNullOrEmpty(line = await reader.ReadLineAsync()))
            {
                var postingModel = GetPostingModel(line);

                if (postingModel.Amount == 0)
                {
                    emptyAmountPostingModel = postingModel;
                }

                transactionBalance += postingModel.Amount;
                transactionModel.Postings.Add(postingModel);
            }

            if (transactionBalance != 0)
            {
                if (emptyAmountPostingModel == null)
                {
                    throw new InvalidOperationException($"The amounts for the transaction did not balance to 0, but instead {transactionBalance:C}.");
                }

                emptyAmountPostingModel.Amount = -transactionBalance;
            }

            return transactionModel;

            PostingModel GetPostingModel(string postingLine)
            {
                var match = PostingLineRegex.Match(postingLine);

                if (!match.Success)
                {
                    throw new ArgumentOutOfRangeException(nameof(postingLine), postingLine, "Unable to parse the posting line.");
                }

                return new PostingModel
                {
                    Account = GetAccountModel(match.Groups["account"].Value),
                    Amount = GetAmount(match.Groups["amount"]) ?? 0,
                };
            }

            static decimal? GetAmount(Capture capture)
            {
                if (capture == null || capture.Value == string.Empty)
                {
                    return null;
                }

                if (decimal.TryParse(capture.Value, out var amount))
                {
                    return amount;
                }

                throw new ArgumentOutOfRangeException($"Unable to parse the amount: '{capture.Value}'.");
            }

            AccountModel GetAccountModel(string accountName)
            {
                return _accounts.GetOrAdd(accountName, new AccountModel { Name = accountName, Id = Guid.NewGuid() });
            }
        }
    }
}