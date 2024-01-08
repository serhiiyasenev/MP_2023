using System.Text;

namespace _11_GOFCuttingShape.Composite
{
    public class Form : IFormComponent
    {
        private string Name;
        private List<IFormComponent> components = new List<IFormComponent>();

        public Form(string name)
        {
            Name = name;
        }

        public void AddComponent(IFormComponent component)
        {
            components.Add(component);
        }

        public string ConvertToString()
        {
            var formContent = new StringBuilder();
            formContent.Append($"<form name='{Name}'>\n");

            foreach (var component in components)
            {
                formContent.AppendLine($"  {component.ConvertToString()}");
            }

            formContent.Append("</form>");
            return formContent.ToString();
        }
    }
}
