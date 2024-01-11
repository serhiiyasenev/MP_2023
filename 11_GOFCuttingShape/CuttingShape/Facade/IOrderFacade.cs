namespace _11_GOFCuttingShape.Facade
{
    public interface IOrderFacade
    {
        void PlaceOrder(int productId, int quantity, string email);
    }
}
