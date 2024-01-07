namespace _11_GOFCuttingShape.Facade
{
    interface IOrderFacade
    {
        void PlaceOrder(string productId, int quantity, string email);
    }
}
