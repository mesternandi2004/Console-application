using CQW1QQ_HSZF_2024251.Application.Interfaces;
using CQW1QQ_HSZF_2024251.Models;
using CQW1QQ_HSZF_2024251.Persistence.MsSql.Interfaces;

namespace CQW1QQ_HSZF_2024251.Application
{
    public class ProductService(IProductRepository productRepository, IPersonRepository personRepository) : IProductService
    {
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IPersonRepository _personRepository = personRepository;

        public event EventHandler<Product>? CriticalStockDetected;
        public event EventHandler<(Person, Product)>? FavoriteProductRestocked;

        public IEnumerable<Product> GetNearExpiryProducts(int daysToExpire)
        {
            var currentDate = DateTime.Now;
            return _productRepository.ReadAll()
                                      .Where(p => p.BestBefore <= currentDate.AddDays(daysToExpire) && p.BestBefore > currentDate)
                                      .ToList();
        }
        public IEnumerable<Product> GetCriticalStockProducts()
        {
            return _productRepository.ReadAll()
                                      .Where(p => p.Quantity <= p.CriticalLevel)
                                      .ToList();
        }
        public void ConsumeProduct(int productId, double quantity)
        {
            var product = _productRepository.Read(productId);
            if (product == null)
            {
                throw new InvalidOperationException("Product not found.");
            }

            product.Quantity -= quantity;
            if (product.Quantity < 0)
            {
                product.Quantity = 0;
            }

            _productRepository.Update(product);

            if (product.Quantity <= product.CriticalLevel)
            {
                OnCriticalStockDetected(product);
            }
        }
        public void RestockProduct(int productId, int quantity)
        {
            var product = _productRepository.Read(productId);
            if (product == null) throw new InvalidOperationException("Product not found.");

            product.Quantity += quantity;
            _productRepository.Update(product);
            NotifyFavoriteProductRestock(product);
        }
        private void NotifyFavoriteProductRestock(Product product)
        {
            var personsToNotify = _personRepository.ReadAll()
                                                   .Where(p => p.FavoriteProductIds.Contains(product.Id));
            foreach (var person in personsToNotify)
            {
                OnFavoriteProductRestocked(person, product);
            }
        }
        protected virtual void OnCriticalStockDetected(Product product)
        {
            CriticalStockDetected?.Invoke(this, product);
        }
        protected virtual void OnFavoriteProductRestocked(Person person, Product product)
        {
            FavoriteProductRestocked?.Invoke(this, (person, product));
        }
    }
}
