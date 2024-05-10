using System.Data.Common;
using System.Data.SqlClient;
using Warehouse.Model;

namespace Warehouse.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly IConfiguration _configuration;

    public WarehouseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    //GDZIE UZYWAMY AWAIT
    //PRZED: 
    //1 -> sqlConnection 
    //2 -> sqlConnection.OpenAsync()
    //3 -> SqlCommand
    //4 -> sqlCommand.ExecuteReaderAsync()
    //5 -> sqlDataReader.ReadAsync()
    //6 -> każda metoda async
    public async Task<int> AddProductToWarehouse(WarehouseRequest request)
    {
        int idOrder = await GetIdOrder(request);
        if (!await CheckWarehouseExists(request.IdWarehouse) ||
            !await CheckProductExists(request.IdProduct) ||
            !await CheckOrderExists(request.IdProduct, request.Amount, request.CreatedAt) ||
            !await IsFulfilled(idOrder))
        {
            throw new InvalidOperationException("Order either does not exist or is already fulfilled.");
        }
        await using SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await sqlConnection.OpenAsync();
        await using SqlCommand sqlCommand = new SqlCommand();
        sqlCommand.Connection = sqlConnection;
        DbTransaction dbTransaction = await sqlConnection.BeginTransactionAsync();
        sqlCommand.Transaction = dbTransaction as SqlTransaction;
        try
        {
            sqlCommand.CommandText = "UPDATE [Order] SET FulfilledAt = GETDATE() WHERE IdProduct = @id";
            sqlCommand.Parameters.AddWithValue("@id", request.IdProduct);
            var affectedRows = await sqlCommand.ExecuteNonQueryAsync();
            if (affectedRows == 0)
            {
                throw new InvalidOperationException("Order either does not exist or is already fulfilled.");
            }
            sqlCommand.Parameters.Clear();
            sqlCommand.CommandText =
                "INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) " +
                "VALUES (@idWarehouse, @id, @idOrder,@Amount, (select price from product where IdProduct=@id), @CreatedAt );SELECT SCOPE_IDENTITY();";
            sqlCommand.Parameters.AddWithValue("@idWarehouse", request.IdWarehouse);
            sqlCommand.Parameters.AddWithValue("@id", request.IdProduct);
            sqlCommand.Parameters.AddWithValue("@idOrder", idOrder);
            sqlCommand.Parameters.AddWithValue("@amount", request.Amount);
            sqlCommand.Parameters.AddWithValue("@createdAt", request.CreatedAt);
            var insertedId = await sqlCommand.ExecuteScalarAsync();
            dbTransaction.Commit();
            return Convert.ToInt32(insertedId);
        }
        catch(SqlException e)
        {
            dbTransaction.Rollback();
            Console.WriteLine(e);
        }
        return -1;
    }

    public async Task<bool> CheckProductExists(int id)
    {
        await using SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await sqlConnection.OpenAsync();
        await using SqlCommand sqlCommand = new SqlCommand();
        sqlCommand.Connection = sqlConnection;
        sqlCommand.CommandText = "SELECT COUNT(1) FROM Product WHERE IdProduct = @id";
        sqlCommand.Parameters.AddWithValue("@id", id);
        var result = await sqlCommand.ExecuteScalarAsync();
        return result is not null;
    }

    public async Task<bool> CheckWarehouseExists(int id)
    {
        await using SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await sqlConnection.OpenAsync();
        await using SqlCommand sqlCommand = new SqlCommand();
        sqlCommand.Connection = sqlConnection;
        sqlCommand.CommandText = "SELECT COUNT(1) FROM Warehouse WHERE IdWarehouse = @id";
        sqlCommand.Parameters.AddWithValue("@id", id);
        var result = await sqlCommand.ExecuteScalarAsync();
        return result is not null;
    }

    public async Task<bool> CheckOrderExists(int id, int amount, DateTime createdAt)
    {
        await using SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await sqlConnection.OpenAsync();
        await using SqlCommand sqlCommand = new SqlCommand();
        sqlCommand.Connection = sqlConnection;
        sqlCommand.CommandText = "SELECT COUNT(1) FROM [Order] WHERE IdProduct = @id AND Amount = @amount AND CreatedAt < @createdAt";
        sqlCommand.Parameters.AddWithValue("@id", id);
        sqlCommand.Parameters.AddWithValue("@amount", amount);
        sqlCommand.Parameters.AddWithValue("@createdAt", createdAt);
        var result = await sqlCommand.ExecuteScalarAsync();
        return result is not null;
    }

    public async Task<bool> IsFulfilled(int id)
    {
        await using SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await sqlConnection.OpenAsync();
        await using SqlCommand sqlCommand = new SqlCommand();
        sqlCommand.Connection = sqlConnection;
        sqlCommand.CommandText = "SELECT COUNT(1) FROM Product_Warehouse WHERE IdOrder = @id";
        sqlCommand.Parameters.AddWithValue("@id", id);
        var result = await sqlCommand.ExecuteScalarAsync();
        return result is not null;
    }

    private async Task<int> GetIdOrder(WarehouseRequest request)
    {
        await using SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await sqlConnection.OpenAsync();
        await using SqlCommand sqlCommand = new SqlCommand();
        sqlCommand.Connection = sqlConnection;
        sqlCommand.CommandText =
            "SELECT IdOrder FROM [Order] WHERE IdProduct = @idProduct AND Amount = @amount AND CreatedAt < @createdAt";
        sqlCommand.Parameters.AddWithValue("@IdProduct", request.IdProduct);
        sqlCommand.Parameters.AddWithValue("@Amount", request.Amount);
        sqlCommand.Parameters.AddWithValue("@CreatedAt", request.CreatedAt);
        var result = await sqlCommand.ExecuteScalarAsync();
        int idOrder = Convert.ToInt32(result);
        return idOrder;
    }
}