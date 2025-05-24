using CQW1QQ_HSZF_2024251.Models;

namespace CQW1QQ_HSZF_2024251.Application.Interfaces
{
    public interface IStorageService
    {
        event EventHandler<string>? CriticalCapacityDetected;

        double GetRemainingCapacity(Storage storage);
        bool PutAwayProduct(ref Product product);

        bool CheckCriticalCapacity(string storageName, double remainingCapacity, double maxCapacity);
    }
}