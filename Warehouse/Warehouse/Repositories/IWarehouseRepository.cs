using Warehouse.Model;

namespace Warehouse.Repositories;

public interface IWarehouseRepository
{
    Task<int> AddProductToWarehouse(WarehouseRequest request);
    Task<bool> CheckProductExists(int id);
    Task<bool> CheckWarehouseExists(int id);
    Task<bool> CheckOrderExists(int id, int amount, DateTime createdAt);
    Task<bool> IsFulfilled(int id);
}