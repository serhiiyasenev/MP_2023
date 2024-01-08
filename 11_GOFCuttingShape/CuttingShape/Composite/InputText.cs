namespace _11_GOFCuttingShape.Composite
{
    public class InputText : IFormComponent
    {
        private string Name { get; }
        private string Value { get; }

        public InputText(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string ConvertToString()
        {
            return $"<inputText name='{Name}' value='{Value}'/>";
        }
    }
}
