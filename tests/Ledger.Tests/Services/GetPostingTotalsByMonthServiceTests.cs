using System;
using NUnit.Framework;

namespace Ledger.Tests.Services
{
    public class GetPostingTotalsByMonthServiceTests : TestBase
    {
        [Test]
        public void ShouldExecute()
        {
            TestBuilder.Begin()
                .AsUser()
                .GetPostingTotalsByMonth(Guid.NewGuid());
        }
    }
}