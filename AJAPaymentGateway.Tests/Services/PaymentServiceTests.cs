using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AJAPaymentGateway.Core.Payments;
using AJAPaymentGateway.DTOs;
using AJAPaymentGateway.Services;
using AJAPaymentGateway.Tests.TestHelpers;
using Moq;
using Xunit;

namespace AJAPaymentGateway.Tests.Services
{
    public class PaymentServiceTests
    {
        [Fact]
        public async Task CreatePayment_ShouldCreateNewPayment()
        {
            // Arrange
            var db = TestDbContextFactory.Create();
            var webhookServiceMock = new Mock<IWebhookService>();
            var processorFactoryMock = new Mock<IPaymentProcessorFactory>();
            var processorMock = new Mock<IPaymentProcessor>();

            processorFactoryMock
                .Setup(f => f.Create())
                .Returns(processorMock.Object);

            var paymentService = new PaymentService(
                db,
                webhookServiceMock.Object,
                processorFactoryMock.Object
            );

            // Act
            var payment = paymentService.CreatePayment(
                orderId: "ORDER-001",
                amount: 150000,
                customerName: "Budi",
                callbackUrl: "https://example.com/callback"
            );

            // Assert
            Assert.NotNull(payment);
            Assert.Equal("pending", payment.Status);
            Assert.Equal(150000, payment.Amount);
            Assert.StartsWith("pay_", payment.PaymentId);
        }

        [Fact]
        public async Task CreatePayment_SameOrderId_ShouldThrow()
        {
            var db = TestDbContextFactory.Create();
            var webhookServiceMock = new Mock<IWebhookService>();
            var processorFactoryMock = new Mock<IPaymentProcessorFactory>();
            var processorMock = new Mock<IPaymentProcessor>();

            processorFactoryMock
                .Setup(f => f.Create())
                .Returns(processorMock.Object);

            var paymentService = new PaymentService(
                db,
                webhookServiceMock.Object,
                processorFactoryMock.Object
            );

            var request = new CreatePaymentRequest
            {
                OrderId = "ORDER-001",
                Amount = 50000,
                CustomerName = "Budi",
                CallbackUrl = "https://merchant.test/webhook"
            };

            await paymentService.CreateAsync(request);

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await paymentService.CreateAsync(request);
            });
        }



    }
}
