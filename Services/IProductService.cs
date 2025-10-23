using AopExample.Attributes;

namespace AopExample.Services;

public interface IProductService
{
    string GetAllProducts();
    string DeleteProduct(int id);
}

public class ProductService : IProductService
{
    /// <summary>
    /// Public method - no AOP interception
    /// </summary>
    public virtual string GetAllProducts()
    {
        return "Products: [Laptop, Phone, Tablet, Monitor]";
    }

    /// <summary>
    /// ════════════════════════════════════════════════════════════════
    /// AOP CONCEPT #2: JOIN POINT (Another Example)
    /// ════════════════════════════════════════════════════════════════
    /// Another example of a JOIN POINT marked with [RequireAdmin].
    /// When this method is called, the AdminRoleInterceptor aspect intercepts it
    /// and applies the authorization advice before allowing the method to execute.
    /// </summary>
    [RequireAdmin]
    public virtual string DeleteProduct(int id)
    {
        return $"Product {id} has been deleted by admin";
    }
}
