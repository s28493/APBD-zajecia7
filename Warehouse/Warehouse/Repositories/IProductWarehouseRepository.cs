using Warehouse.Models;

namespace Warehouse.Repositories;

public interface IProductWarehouseRepository
{
    int AddProductToWarehouse(WarehouseRquest request);
}