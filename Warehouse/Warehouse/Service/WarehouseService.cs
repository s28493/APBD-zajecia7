using Warehouse.Model;
using Warehouse.Repositories;

namespace Warehouse.Service;

public class WarehouseService : IWarehouseService
{
    private readonly IWarehouseRepository _warehouseRepository;
    
    public WarehouseService(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }
    
    public async Task<int> AddProductToWarehouse(WarehouseRequest request)
    {
        int insertedId = await _warehouseRepository.AddProductToWarehouse(request);
        return insertedId;
    }
}