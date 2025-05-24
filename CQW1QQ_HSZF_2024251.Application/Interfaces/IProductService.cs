using CQW1QQ_HSZF_2024251.Models;

namespace CQW1QQ_HSZF_2024251.Application.Interfaces
{
    public interface IProductService
    {
        event EventHandler<Product>? CriticalStockDetected;
        event EventHandler<(Person, Product)>? FavoriteProductRestocked;

        void ConsumeProduct(int productId, double quantity);
        IEnumerable<Product> GetCriticalStockProducts();
        IEnumerable<Product> GetNearExpiryProducts(int daysToExpire);
        void RestockProduct(int productId, int quantity);
    }
}