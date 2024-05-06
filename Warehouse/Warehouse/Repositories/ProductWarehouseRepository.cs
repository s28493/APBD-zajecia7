using System.Data.SqlClient;
using Warehouse.Models;

namespace Warehouse.Repositories;

public class ProductWarehouseRepository : IProductWarehouseRepository
{
    private readonly IConfiguration _configuration;

    public ProductWarehouseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public int AddProductToWarehouse(WarehouseRquest request)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        decimal price = request.Amount * GetProductPrice(request.IdProduct);

        SqlCommand command = new SqlCommand(
                "INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) " +
                "VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, @CreatedAt); " +
                "SELECT SCOPE_IDENTITY();", connection);

        command.Parameters.AddWithValue("@IdWarehouse", request.IdWarehouse);
        command.Parameters.AddWithValue("@IdProduct", request.IdProduct);
        command.Parameters.AddWithValue("@IdOrder", request.IdOrder);
        command.Parameters.AddWithValue("@Amount", request.Amount);
        command.Parameters.AddWithValue("@Price", price); 
        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    private decimal GetProductPrice(int productId)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        SqlCommand command = new SqlCommand(
                "SELECT Price FROM Product WHERE IdProduct = @ProductId", connection);
        command.Parameters.AddWithValue("@ProductId", productId);

        return Convert.ToDecimal(command.ExecuteScalar());
    }
}