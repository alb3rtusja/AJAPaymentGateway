using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AJAPaymentGateway.Services;
using AJAPaymentGateway.Tests.TestHelpers;
using Xunit;

namespace AJAPaymentGateway.Tests.Services
{
    public class IdempotencyServiceTests
    {
        [Fact]
        public void SaveAndGet_ShouldReturnSameRecord()
        {
            var db = TestDbContextFactory.Create();
            var service = new IdempotencyService(db);

            service.Save("key-1", "hash123", 201, "{ }");

            var record = service.Get("key-1");

            Assert.NotNull(record);
            Assert.Equal("hash123", record!.RequestHash);
        }
    }
}
