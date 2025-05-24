
using CQW1QQ_HSZF_2024251.Application.Interfaces;
using CQW1QQ_HSZF_2024251.Models;
using CQW1QQ_HSZF_2024251.Persistence.MsSql.Interfaces;
using CQW1QQ_HSZF_2024251.Persistence.MsSql.Repository.Interfaces;

namespace CQW1QQ_HSZF_2024251.Application
{
    public class StorageService : IStorageService
    {
        public event EventHandler<string>? CriticalCapacityDetected;

        private readonly IFridgeRepository _fridgeRepository;
        private readonly IPantryRepository _pantryRepository;

        public StorageService(IFridgeRepository fridgeRepository, IPantryRepository pantryRepository)
        {
            _fridgeRepository = fridgeRepository;
            _pantryRepository = pantryRepository;
        }

        public bool PutAwayProduct(ref Product product)
        {
            if (product.StoreInFridge)
            {
                var fridge = _fridgeRepository.ReadAll().FirstOrDefault();
                double remainingCapacity = GetRemainingCapacity(fridge);
                CheckCriticalCapacity("Fridge", remainingCapacity, fridge.Capacity);

                if (remainingCapacity - product.Quantity >= 0)
                {
                    fridge.ProductIds.Add(product.Id);
                    fridge.Products.Add(product);

                    product.Storage = fridge;
                    return true;
                }
            }
            else
            {
                var pantry = _pantryRepository.ReadAll().FirstOrDefault();
                double remainingCapacity = GetRemainingCapacity(pantry);
                CheckCriticalCapacity("Pantry", remainingCapacity, pantry.Capacity);

                if (remainingCapacity - product.Quantity >= 0)
                {
                    pantry.ProductIds.Add(product.Id);
                    pantry.Products.Add(product);
                    product.Storage = pantry;

                    return true;
                }
            }
            return false;
        }

        public bool CheckCriticalCapacity(string storageName, double remainingCapacity, double maxCapacity)
        {
            if (remainingCapacity / maxCapacity < 0.1)
            {
                OnCriticalCapacityDetected(storageName);
                return true;
            }
            return false;
        }
        public double GetRemainingCapacity(Storage storage)
        {
            double totalUsedCapacity = storage.Products.Sum(p => p.Quantity);
            return storage.Capacity - totalUsedCapacity;
        }
        protected virtual void OnCriticalCapacityDetected(string storageName)
        {
            CriticalCapacityDetected?.Invoke(this, storageName);
        }
    }
}
