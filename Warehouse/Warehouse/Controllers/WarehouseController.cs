using Microsoft.AspNetCore.Mvc;
using Warehouse.Models;
using Warehouse.Services;

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
    public IActionResult AddProductToWarehouse(WarehouseRquest request)
    {
        int affectedCount = _warehouseService.AddProductToWarehouse(request);
        return StatusCode(StatusCodes.Status201Created);
    }
}