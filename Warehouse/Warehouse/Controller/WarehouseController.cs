using Microsoft.AspNetCore.Mvc;
using Warehouse.Model;
using Warehouse.Service;

namespace Warehouse.Controllers;

[ApiController]
[Route("[controller]")]
public class WarehouseController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;

    public WarehouseController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    [HttpPost]
    public async Task<IActionResult> AddProductToWarehouse(WarehouseRequest request)
    {
        int id = await _warehouseService.AddProductToWarehouse(request);
        return Ok("Wstawione id to: " + id);
    }
}