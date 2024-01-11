namespace _11_GOFCuttingShape.Facade.Products
{
    internal class ProductCatalog : IProductCatalog
    {
        private List<Product> _products = new List<Product>();

        public ProductCatalog()
        {
            _products.Add(new Product { ProductId = 1, Name = "A", Price = 123 });
            _products.Add(new Product { ProductId = 2, Name = "B", Price = 234 });
            _products.Add(new Product { ProductId = 3, Name = "C", Price = 345 });
        }

        public Product GetProductDetails(int productId)
        {
            var product = _products.FirstOrDefault(e => e.ProductId.Equals(productId));

            if (product != null)
            {
                return product;
            }
            else
            {
                throw new ArgumentException($"Product with id `{productId}` not found");
            }
        }
    }
}
