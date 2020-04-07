using System;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Ledger.WebApi.Concept;
using Ledger.WebApi.Models;

namespace Ledger.WebApi.Services
{
    public interface IInsertTransactionService
    {
        Task<Guid> ExecuteAsync(TransactionModel transactionModel, CancellationToken cancellationToken);
    }

    public class InsertTransactionService : IInsertTransactionService
    {
        private readonly IRequestContext _requestContext;
        private readonly IRepository _repository;

        public InsertTransactionService(IRequestContext requestContext, IRepository repository)
        {
            _requestContext = requestContext;
            _repository = repository;
        }

        public async Task<Guid> ExecuteAsync(TransactionModel transactionModel, CancellationToken cancellationToken)
        {
            transactionModel.Id = Guid.NewGuid();

            const string sql = @"
insert into [dbo].[Transaction] (Id, UserId, Description, Amount, PostedDate, CreatedTimestamp, UpdatedTimestamp) values
    (@Id, @UserId, @Description, @Amount, @PostedDate, @CurrentTimestamp, @CurrentTimestamp)";

            var parameters = new DynamicParameters();
            parameters.Add("Id", transactionModel.Id);
            parameters.Add("UserId", _requestContext.UserId);
            parameters.Add("Description", transactionModel.Description);
            parameters.Add("Amount", transactionModel.Amount);
            parameters.Add("PostedDate", transactionModel.PostedDate);
            parameters.Add("CurrentTimestamp", DateTime.UtcNow);

            var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);

            await _repository.ExecuteAsync(command);

            return transactionModel.Id;
        }
    }
}