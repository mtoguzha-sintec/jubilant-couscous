using AopExample.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace AopExample.Controllers;

/// <summary>
/// Admin-only controller - all methods require admin role via AOP
/// The [RequireAdmin] attribute at class level protects ALL methods
/// </summary>
[ApiController]
[Route("api/[controller]")]
[RequireAdmin]  // Applied at controller level - protects all actions
public class AdminController : ControllerBase
{
    /// <summary>
    /// Get system settings - admin only (inherited from controller attribute)
    /// </summary>
    [HttpGet("settings")]
    public virtual IActionResult GetSettings()
    {
        return Ok(new 
        { 
            success = true, 
            data = new 
            {
                systemName = "AOP Example System",
                version = "1.0.0",
                environment = "Development",
                features = new[] { "AOP", "Autofac", "Interceptors" }
            }
        });
    }

    /// <summary>
    /// Get all system logs - admin only (inherited from controller attribute)
    /// </summary>
    [HttpGet("logs")]
    public virtual IActionResult GetLogs()
    {
        return Ok(new 
        { 
            success = true, 
            data = new[] 
            {
                new { timestamp = DateTime.Now.AddMinutes(-10), level = "INFO", message = "Application started" },
                new { timestamp = DateTime.Now.AddMinutes(-5), level = "WARN", message = "High memory usage detected" },
                new { timestamp = DateTime.Now, level = "INFO", message = "Request processed" }
            }
        });
    }

    /// <summary>
    /// Reset system - admin only (inherited from controller attribute)
    /// </summary>
    [HttpPost("reset")]
    public virtual IActionResult ResetSystem()
    {
        return Ok(new 
        { 
            success = true, 
            message = "System has been reset by admin" 
        });
    }
}
