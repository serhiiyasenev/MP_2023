using _11_GOFCuttingShape.Facade.Payments;

namespace _11_GOFCuttingShape.Facade.Invoices
{
    interface IInvoiceSystem
    {
        void SendInvoice(Payment invoice);
    }
}
