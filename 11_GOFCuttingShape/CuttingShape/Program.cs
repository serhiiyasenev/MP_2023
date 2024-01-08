using _11_GOFCuttingShape.Adapter.Elements;
using _11_GOFCuttingShape.Adapter;
using _11_GOFCuttingShape.Composite;
using _11_GOFCuttingShape.Facade.Invoices;
using _11_GOFCuttingShape.Facade.Payments;
using _11_GOFCuttingShape.Facade.Products;
using _11_GOFCuttingShape.Facade;

namespace _11_GOFCuttingShape
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 1. Adapter
            Console.WriteLine("-----Adapter Start-----\n");
            var myElements = new MyElements();
            var adapter = new ElementsAdapter<string>(myElements);
            var printer = new Printer();
            printer.Print(adapter);
            Console.WriteLine("\n-----Adapter End-----\n");

            // 2. Composite 1
            Console.WriteLine("-----Composite 1 Start-----\n");
            var inputText = new InputText("Input Name", "Input Value");
            Console.WriteLine(inputText.ConvertToString());
            var labelText = new LabelText("Label 1");
            Console.WriteLine(labelText.ConvertToString());
            Console.WriteLine("\n-----Composite 1 End-----\n");

            // 3. Compostie 2
            Console.WriteLine("-----Composite 2 Start-----\n");
            var form = new Form("New Form");
            form.AddComponent(new LabelText("Label 2"));
            form.AddComponent(new InputText("Input 2", "New Input Value"));
            Console.WriteLine(form.ConvertToString());
            Console.WriteLine("\n-----Composite 2 End-----\n");

            // 4. Facade
            Console.WriteLine("-----Facade Start-----\n");
            IProductCatalog productCatalog = new ProductCatalog();
            IPaymentSystem paymentSystem = new PaymentSystem();
            IInvoiceSystem invoiceSystem = new InvoiceSystem();

            OrderFacade orderFacade = new OrderFacade(productCatalog, paymentSystem, invoiceSystem);

            try
            {
                orderFacade.PlaceOrder(productId: 1, quantity: 2, email: "customer1@example.com");
                orderFacade.PlaceOrder(productId: 2, quantity: 4, email: "customer2@example.com");
                orderFacade.PlaceOrder(productId: 3, quantity: 6, email: "customer3@example.com");

                Console.WriteLine("\nOrders placed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Order failed: {ex.Message}");
            }
            Console.WriteLine("\n-----Facade End-----\n");

            Console.ReadLine();
        }
    }
}