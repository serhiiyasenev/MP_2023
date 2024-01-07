using _11_GOFCuttingShape.Adapter.Elements;
using _11_GOFCuttingShape.Adapter;

namespace _11_GOFCuttingShape
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var myElements = new MyElements();
            var adapter = new ElementsAdapter<string>(myElements);

            var printer = new Printer();
            printer.Print(adapter);

            Console.ReadLine();
        }
    }
}