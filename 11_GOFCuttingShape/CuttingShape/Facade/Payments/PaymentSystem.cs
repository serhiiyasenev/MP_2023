namespace _11_GOFCuttingShape.Facade.Payments
{
    internal class PaymentSystem : IPaymentSystem
    {
        public bool MakePayment(Payment payment)
        {
            Console.WriteLine($"The payment `{payment.PaymentId}` has been made with amount `{payment.Amount}`.");
            return true;
        }
    }
}
