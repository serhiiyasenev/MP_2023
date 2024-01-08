namespace _11_GOFCuttingShape.Facade.Invoices
{
    internal class InvoiceSystem : IInvoiceSystem
    {
        public void SendInvoice(Invoice invoice)
        {
            Console.WriteLine($"Invoice `{invoice.InvoiceId}` with amount `{invoice.Amount}` was sent to email: `{invoice.Email}`.");
        }
    }
}
