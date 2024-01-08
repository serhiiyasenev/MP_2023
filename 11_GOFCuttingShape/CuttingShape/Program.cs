using _11_GOFCuttingShape.Adapter.Elements;
using _11_GOFCuttingShape.Adapter;
using _11_GOFCuttingShape.Composite;

namespace _11_GOFCuttingShape
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 1. Adapter
            var myElements = new MyElements();
            var adapter = new ElementsAdapter<string>(myElements);
            var printer = new Printer();
            printer.Print(adapter);

            // 2. Composite
            var inputText = new InputText("Input Name", "Input Value");
            Console.WriteLine(inputText.ConvertToString());
            var labelText = new LabelText("Label 1");
            Console.WriteLine(labelText.ConvertToString());




            Console.ReadLine();
        }
    }
}