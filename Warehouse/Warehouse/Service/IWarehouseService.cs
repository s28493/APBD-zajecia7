using Warehouse.Model;

namespace Warehouse.Service;

public interface IWarehouseService
{
    Task<int> AddProductToWarehouse(WarehouseRequest request);
}