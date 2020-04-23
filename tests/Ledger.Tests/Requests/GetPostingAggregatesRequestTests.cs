using System;
using Ledger.WebApi.Requests;
using NUnit.Framework;

namespace Ledger.Tests.Requests
{
    public class GetPostingAggregatesRequestTests : TestBase
    {
        [Test]
        public void ShouldExecute()
        {
            TestBuilder.Begin()
                .AsUser()
                .ExecuteRequest(new GetPostingAggregatesRequest(Guid.NewGuid()));
        }
    }
}