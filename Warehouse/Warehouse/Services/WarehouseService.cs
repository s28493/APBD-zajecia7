using Warehouse.Models;
using Warehouse.Repositories;

namespace Warehouse.Services;

public class WarehouseService : IWarehouseService
{
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductWarehouseRepository _productWarehouseRepository;

    public WarehouseService(IProductRepository productRepository, IOrderRepository orderRepository, IProductWarehouseRepository productWarehouseRepository)
    {
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _productWarehouseRepository = productWarehouseRepository;
    }

    public int AddProductToWarehouse(WarehouseRquest request)
    {
        Product product = _productRepository.GetProductById(request.IdProduct);
        if (product == null)
        {
            throw new ArgumentException("Product not found.");
        }
        
        if (!_orderRepository.DoesOrderExist(request.IdProduct, request.Amount, request.CreatedAt))
        {
            throw new ArgumentException("Order not found or invalid creation date.");
        }
        
        if (_orderRepository.IsOrderFulfilled(request.IdOrder))
        {
            throw new InvalidOperationException("Order has already been fulfilled.");
        }
        
        _orderRepository.UpdateOrderFulfilledAt(request.IdOrder);
        
        return _productWarehouseRepository.AddProductToWarehouse(request);
    }
}