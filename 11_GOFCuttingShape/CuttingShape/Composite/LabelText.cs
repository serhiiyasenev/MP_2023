namespace _11_GOFCuttingShape.Composite
{
    public class LabelText : IFormComponent
    {
        private string Value { get; }

        public LabelText(string value)
        {
            Value = value;
        }

        public string ConvertToString()
        {
            return $"<label value='{Value}'/>";
        }
    }

}
