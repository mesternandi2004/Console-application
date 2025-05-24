using Moq;
using CQW1QQ_HSZF_2024251.Models;
using NUnit.Framework;
using CQW1QQ_HSZF_2024251.Persistence.MsSql.Interfaces;
using CQW1QQ_HSZF_2024251.Application;

namespace CQW1QQ_HSZF_2024251.Test
{

    [TestFixture]
    public class ProductServiceTests
    {
        private Mock<IProductRepository> _productRepositoryMock;
        private Mock<IPersonRepository> _personRepositoryMock;
        private ProductService _productService;

        [SetUp]
        public void SetUp()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _personRepositoryMock = new Mock<IPersonRepository>();
            _productService = new ProductService(_productRepositoryMock.Object, _personRepositoryMock.Object);
        }

        [Test]
        public void GetNearExpiryProducts_ShouldReturnProductsExpiringSoon()
        {
            var today = DateTime.Now;
            var products = new List<Product>
        {
            new Product(1, "Milk", 2, 1, today.AddDays(3), true),
            new Product(2, "Bread", 5, 2, today.AddDays(10), false)
        };
            _productRepositoryMock.Setup(repo => repo.ReadAll()).Returns(products);


            var result = _productService.GetNearExpiryProducts(5);

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Milk"));
        }

        [Test]
        public void ConsumeProduct_ShouldReduceQuantityAndTriggerCriticalStockEvent()
        {

            var product = new Product(1, "Milk", 5, 3, DateTime.Now.AddDays(10), true);
            _productRepositoryMock.Setup(repo => repo.Read(1)).Returns(product);
            bool eventTriggered = false;
            _productService.CriticalStockDetected += (sender, e) => eventTriggered = true;


            _productService.ConsumeProduct(1, 3);

            _productRepositoryMock.Verify(repo => repo.Update(product), Times.Once);
            Assert.That(product.Quantity, Is.EqualTo(2));
            Assert.That(eventTriggered, Is.True);
        }

        [Test]
        public void RestockProduct_ShouldIncreaseQuantityAndNotifyFavoriteProductRestock()
        {
            var product = new Product(1, "Milk", 2, 1, DateTime.Now.AddDays(10), true);
            var person = new Person("John", new List<int> { 1 }, true, new List<Product>());
            _productRepositoryMock.Setup(repo => repo.Read(1)).Returns(product);
            _personRepositoryMock.Setup(repo => repo.ReadAll()).Returns(new List<Person> { person });

            bool eventTriggered = false;
            _productService.FavoriteProductRestocked += (sender, e) => eventTriggered = true;


            _productService.RestockProduct(1, 5);

            _productRepositoryMock.Verify(repo => repo.Update(product), Times.Once);
            Assert.That(product.Quantity, Is.EqualTo(7));
            Assert.That(eventTriggered, Is.True);
        }

        [Test]
        public void RestockProduct_ShouldNotNotifyIfNoFavoritePersonExists()
        {
            var product = new Product(1, "Butter", 5, 2, DateTime.Now.AddDays(10), true);
            _productRepositoryMock.Setup(repo => repo.Read(1)).Returns(product);
            _personRepositoryMock.Setup(repo => repo.ReadAll()).Returns(new List<Person>());

            bool eventTriggered = false;
            _productService.FavoriteProductRestocked += (sender, e) => eventTriggered = true;

            _productService.RestockProduct(1, 5);

            Assert.That(eventTriggered, Is.False);
        }

        [Test]
        public void ConsumeProduct_ShouldNotReduceQuantityIfProductNotFound()
        {
            _productRepositoryMock.Setup(repo => repo.Read(1)).Returns((Product)null);

            Assert.Throws<InvalidOperationException>(() => _productService.ConsumeProduct(1, 3));
        }

        [Test]
        public void GetCriticalStockProducts_ShouldReturnCriticalProducts()
        {
            var products = new List<Product>
                {
                    new Product(1, "Milk", 1, 2, DateTime.Now.AddDays(5), true),
                    new Product(2, "Bread", 5, 1, DateTime.Now.AddDays(10), false)
                };

            _productRepositoryMock.Setup(repo => repo.ReadAll()).Returns(products);

            var result = _productService.GetCriticalStockProducts();

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Milk"));
        }
    }

    [TestFixture]
    public class StorageServiceTests
    {
        private Mock<IFridgeRepository> _fridgeRepositoryMock;
        private Mock<IPantryRepository> _pantryRepositoryMock;
        private StorageService _storageService;

        [SetUp]
        public void SetUp()
        {
            _fridgeRepositoryMock = new Mock<IFridgeRepository>();
            _pantryRepositoryMock = new Mock<IPantryRepository>();
            _storageService = new StorageService(_fridgeRepositoryMock.Object, _pantryRepositoryMock.Object);
        }



        [Test]
        public void PutAwayProduct_ShouldStoreProductInFridgeIfEnoughSpace()
        {

            var fridge = new Fridge(10)
            {
                Products = new List<Product>(),
                ProductIds = new List<int>()
            };
            var product = new Product(1, "Butter", 3, 1, DateTime.Now.AddDays(10), true);

            _fridgeRepositoryMock.Setup(repo => repo.ReadAll()).Returns(new List<Fridge> { fridge });

            var result = _storageService.PutAwayProduct(ref product);

            Assert.That(result, Is.True);
            Assert.That(product.Storage, Is.EqualTo(fridge));
            Assert.That(fridge.Products, Contains.Item(product));
        }

        [Test]
        public void GetRemainingCapacity_ShouldReturnCorrectValue()
        {
            var fridge = new Fridge(20)
            {
                ProductIds = new List<int> { 1, 2 },
                Products = new List<Product>
                {
                   new Product(1, "Butter", 5, 1, DateTime.Now.AddDays(10), true),
                   new Product(2, "Cheese", 7, 1, DateTime.Now.AddDays(10), true)
                }


            };
            var remainingCapacity = _storageService.GetRemainingCapacity(fridge);

            Assert.That(remainingCapacity, Is.EqualTo(8));
        }

        [Test]
        public void PutAwayProduct_ShouldNotStoreIfInsufficientCapacity()
        {
            var pantry = new Pantry(5)
            {
                Products = new List<Product> { new Product(2, "Rice", 5, 2, DateTime.Now.AddDays(30), false) }
            };

            var product = new Product(1, "Beans", 3, 1, DateTime.Now.AddDays(5), false);

            _pantryRepositoryMock.Setup(repo => repo.ReadAll()).Returns(new List<Pantry> { pantry });

            var result = _storageService.PutAwayProduct(ref product);

            Assert.That(result, Is.False);
            Assert.That(product.Storage, Is.Null);
        }

        [Test]
        public void PutAwayProduct_ShouldStoreProductInPantryIfEnoughSpace()
        {
            var pantry = new Pantry(10) { Products = new List<Product>(), ProductIds = new List<int>() };
            var product = new Product(1, "Beans", 2, 1, DateTime.Now.AddDays(5), false);

            _pantryRepositoryMock.Setup(repo => repo.ReadAll()).Returns(new List<Pantry> { pantry });

            var result = _storageService.PutAwayProduct(ref product);

            Assert.That(result, Is.True);
            Assert.That(product.Storage, Is.EqualTo(pantry));
        }


    }

}

