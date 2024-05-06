using System.Data.SqlClient;
using Warehouse.Models;

namespace Warehouse.Repositories;

public class ProductRepositoriy : IProductRepository
{
    private readonly IConfiguration _configuration;

    public ProductRepositoriy(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Product GetProductById(int productId)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        using SqlCommand command = new SqlCommand(
            "SELECT IdProduct, Name, Description, Price FROM Product WHERE IdProduct = @ProductId", connection);
        command.Parameters.AddWithValue("@ProductId", productId);

        SqlDataReader reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Product
            {
                IdProduct = Convert.ToInt32(reader["IdProduct"]),
                Name = reader["Name"].ToString(),
                Description = reader["Description"].ToString(),
                Price = Convert.ToDecimal(reader["Price"])
            };
        }
        else
        {
            return null; // Produkt o podanym Id nie został znaleziony
        }
    }

    public decimal GetProductPrice(int productId)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        using SqlCommand command = new SqlCommand(
            "SELECT Price FROM Product WHERE IdProduct = @ProductId", connection);
        command.Parameters.AddWithValue("@ProductId", productId);

        object result = command.ExecuteScalar();
        if (result != null)
        {
            return Convert.ToDecimal(result);
        }
        else
        {
            throw new InvalidOperationException("Product not found.");
        }
    }
}
