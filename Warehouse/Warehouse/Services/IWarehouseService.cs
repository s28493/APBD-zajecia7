using Warehouse.Models;

namespace Warehouse.Services;

public interface IWarehouseService
{
    public int AddProductToWarehouse(WarehouseRquest request);
}