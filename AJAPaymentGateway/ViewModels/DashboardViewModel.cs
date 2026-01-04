namespace AJAPaymentGateway.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalPayments { get; set; }
        public int PendingPayments { get; set; }
        public int SuccessPayments { get; set; }
        public int FailedPayments { get; set; }
    }
}
