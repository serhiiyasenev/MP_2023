using _11_GOFCuttingShape.Facade.Invoices;
using _11_GOFCuttingShape.Facade.Payments;
using _11_GOFCuttingShape.Facade.Products;

namespace _11_GOFCuttingShape.Facade
{
    public class OrderFacade : IOrderFacade
    {
        private readonly IProductCatalog _productCatalog;
        private readonly IPaymentSystem _paymentSystem;
        private readonly IInvoiceSystem _invoiceSystem;

        public OrderFacade(IProductCatalog productCatalog, IPaymentSystem paymentSystem, IInvoiceSystem invoiceSystem)
        {
            _productCatalog = productCatalog;
            _paymentSystem = paymentSystem;
            _invoiceSystem = invoiceSystem;
        }

        public void PlaceOrder(int productId, int quantity, string email)
        {
            var product = _productCatalog.GetProductDetails(productId);
            var amount = product.Price * quantity;
            var payment = new Payment { Amount = amount };

            if (_paymentSystem.MakePayment(payment))
            {
                var invoice = new Invoice { Email = email, Amount = amount };
                _invoiceSystem.SendInvoice(invoice);
            }
            else
            {
                throw new Exception("Payment failed.");
            }
        }
    }
}
