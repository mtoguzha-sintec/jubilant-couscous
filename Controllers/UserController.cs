using AopExample.Services;
using Microsoft.AspNetCore.Mvc;

namespace AopExample.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Gets all users - requires admin role (enforced via AOP)
    /// To test: Add header "X-User-Role: Admin" to access this endpoint
    /// </summary>
    [HttpGet("all")]
    public IActionResult GetAllUsers()
    {
        try
        {
            var users = _userService.GetAllUsers();
            return Ok(new { success = true, data = users });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Gets public data - no admin role required
    /// </summary>
    [HttpGet("public")]
    public IActionResult GetPublicData()
    {
        var data = _userService.GetPublicData();
        return Ok(new { success = true, data = data });
    }
}
