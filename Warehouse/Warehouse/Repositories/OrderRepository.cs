using System.Data.SqlClient;

namespace Warehouse.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly IConfiguration _configuration;

    public OrderRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public bool DoesOrderExist(int productId, int amount, DateTime createdAt)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
            
        using SqlCommand command = new SqlCommand(
                "SELECT COUNT(*) FROM [Order] WHERE IdProduct = @ProductId AND Amount = @Amount " +
                "AND CreatedAt < @CreatedAt", connection);
        command.Parameters.AddWithValue("@ProductId", productId);
        command.Parameters.AddWithValue("@Amount", amount);
        command.Parameters.AddWithValue("@CreatedAt", createdAt);

        return (int)command.ExecuteScalar() > 0;
    }

    public bool IsOrderFulfilled(int orderId)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        using SqlCommand command = new SqlCommand(
                "SELECT COUNT(*) FROM [Order] WHERE IdOrder = @OrderId AND FulfilledAt IS NOT NULL", connection);
            command.Parameters.AddWithValue("@OrderId", orderId);

        return (int)command.ExecuteScalar() > 0;
    }

    public void UpdateOrderFulfilledAt(int orderId)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        using SqlCommand command = new SqlCommand(
                "UPDATE [Order] SET FulfilledAt = @FulfilledAt WHERE IdOrder = @OrderId", connection);
        command.Parameters.AddWithValue("@FulfilledAt", DateTime.Now);
        command.Parameters.AddWithValue("@OrderId", orderId);

        command.ExecuteNonQuery();
    }
}