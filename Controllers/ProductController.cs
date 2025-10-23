using AopExample.Services;
using Microsoft.AspNetCore.Mvc;

namespace AopExample.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Gets all products - public access
    /// </summary>
    [HttpGet]
    public IActionResult GetAllProducts()
    {
        var products = _productService.GetAllProducts();
        return Ok(new { success = true, data = products });
    }

    /// <summary>
    /// Deletes a product - requires admin role (enforced via AOP)
    /// To test: Add header "X-User-Role: Admin" to access this endpoint
    /// </summary>
    [HttpDelete("{id}")]
    public IActionResult DeleteProduct(int id)
    {
        try
        {
            var result = _productService.DeleteProduct(id);
            return Ok(new { success = true, message = result });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { success = false, message = ex.Message });
        }
    }
}
