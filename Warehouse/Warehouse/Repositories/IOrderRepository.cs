namespace Warehouse.Repositories;

public interface IOrderRepository
{
    bool DoesOrderExist(int productId, int amount, DateTime createdAt);
    bool IsOrderFulfilled(int orderId);
    void UpdateOrderFulfilledAt(int orderId);
}