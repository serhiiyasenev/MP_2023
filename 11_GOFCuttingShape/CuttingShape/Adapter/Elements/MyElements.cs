namespace _11_GOFCuttingShape.Adapter.Elements
{
    public class MyElements : IElements<string>
    {
        public IEnumerable<string> GetElements()
        {
            return new List<string> { "Element 1", "Element 2", "Element 3" };
        }
    }
}
