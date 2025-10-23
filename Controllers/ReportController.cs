using AopExample.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace AopExample.Controllers;

/// <summary>
/// Mixed access controller - some methods require admin, some are public
/// Demonstrates method-level [RequireAdmin] attribute on controllers
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    /// <summary>
    /// Get public report - accessible to everyone
    /// </summary>
    [HttpGet("public")]
    public virtual IActionResult GetPublicReport()
    {
        return Ok(new 
        { 
            success = true, 
            data = new 
            {
                reportName = "Public Sales Report",
                totalSales = 50000,
                period = "Q3 2025"
            }
        });
    }

    /// <summary>
    /// Get detailed report - requires admin role (method-level attribute)
    /// </summary>
    [HttpGet("detailed")]
    [RequireAdmin]  // Only this method requires admin
    public virtual IActionResult GetDetailedReport()
    {
        return Ok(new 
        { 
            success = true, 
            data = new 
            {
                reportName = "Detailed Sales Report",
                totalSales = 50000,
                profit = 15000,
                costs = 35000,
                breakdown = new[] 
                {
                    new { product = "Laptop", sales = 20000, profit = 6000 },
                    new { product = "Phone", sales = 18000, profit = 5400 },
                    new { product = "Tablet", sales = 12000, profit = 3600 }
                },
                period = "Q3 2025"
            }
        });
    }

    /// <summary>
    /// Get financial report - requires admin role (method-level attribute)
    /// </summary>
    [HttpGet("financial")]
    [RequireAdmin]  // Only this method requires admin
    public virtual IActionResult GetFinancialReport()
    {
        return Ok(new 
        { 
            success = true, 
            data = new 
            {
                reportName = "Financial Report",
                revenue = 50000,
                expenses = 35000,
                netProfit = 15000,
                tax = 3000,
                period = "Q3 2025"
            }
        });
    }

    /// <summary>
    /// Download report - accessible to everyone
    /// </summary>
    [HttpGet("download/{reportId}")]
    public virtual IActionResult DownloadReport(int reportId)
    {
        return Ok(new 
        { 
            success = true, 
            message = $"Report {reportId} is being downloaded",
            downloadUrl = $"/downloads/report-{reportId}.pdf"
        });
    }
}
