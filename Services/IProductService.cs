using AopExample.Attributes;

namespace AopExample.Services;

public interface IProductService
{
    string GetAllProducts();
    string DeleteProduct(int id);
}

public class ProductService : IProductService
{
    public virtual string GetAllProducts()
    {
        return "Products: [Laptop, Phone, Tablet, Monitor]";
    }

    [RequireAdmin]
    public virtual string DeleteProduct(int id)
    {
        return $"Product {id} has been deleted by admin";
    }
}
