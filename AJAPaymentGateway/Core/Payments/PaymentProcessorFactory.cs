namespace AJAPaymentGateway.Core.Payments
{
    public class PaymentProcessorFactory
    {
        private readonly IConfiguration _config;
        private readonly IServiceProvider _provider;

        public PaymentProcessorFactory(
            IConfiguration config,
            IServiceProvider provider)
        {
            _config = config;
            _provider = provider;
        }

        public IPaymentProcessor Create()
        {
            var mode = _config["PaymentGateway:Mode"];

            return mode == "Production"
                ? _provider.GetRequiredService<ProductionPaymentProcessor>()
                : _provider.GetRequiredService<SandboxPaymentProcessor>();
        }
    }
}
