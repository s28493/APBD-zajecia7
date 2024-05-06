using Warehouse.Models;

namespace Warehouse.Repositories;

public interface IProductRepository
{
    Product GetProductById(int productId);
    decimal GetProductPrice(int productId);
}