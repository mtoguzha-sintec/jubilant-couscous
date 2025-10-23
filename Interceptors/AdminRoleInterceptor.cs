using System.Reflection;
using AopExample.Attributes;
using Castle.DynamicProxy;

namespace AopExample.Interceptors;

/// <summary>
/// Interceptor that checks if user has admin role before executing methods marked with RequireAdminAttribute
/// 
/// ════════════════════════════════════════════════════════════════
/// AOP CONCEPT #1: ASPECT
/// ════════════════════════════════════════════════════════════════
/// This class is the ASPECT - it encapsulates the cross-cutting concern of authorization/security
/// that needs to be applied across multiple classes and methods in the application.
/// Instead of duplicating authorization code in every method, this aspect centralizes it.
/// </summary>
public class AdminRoleInterceptor : IInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AdminRoleInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// ════════════════════════════════════════════════════════════════
    /// AOP CONCEPT #3: ADVICE (Around Advice - Before implementation)
    /// ════════════════════════════════════════════════════════════════
    /// This method contains the ADVICE - the code that executes at join points.
    /// This is "around" advice because it wraps method execution and controls WHEN/IF to proceed.
    /// Currently implements "before" logic, but could also add "after" logic post-Proceed().
    /// </summary>
    public void Intercept(IInvocation invocation)
    {
        // ════════════════════════════════════════════════════════════════
        // AOP CONCEPT #4: POINTCUT (Matching Expression)
        // ════════════════════════════════════════════════════════════════
        // Check if the method has RequireAdminAttribute - this determines if advice should be applied
        var methodInfo = invocation.MethodInvocationTarget ?? invocation.Method;
        var hasRequireAdminAttribute = methodInfo.GetCustomAttribute<RequireAdminAttribute>() != null ||
                                       invocation.TargetType?.GetCustomAttribute<RequireAdminAttribute>() != null;

        if (hasRequireAdminAttribute)
        {
            // ═════ BEFORE ADVICE: Execute logic BEFORE the actual method ═════
            Console.WriteLine($"[AOP Interceptor] Checking admin role for method: {methodInfo.Name}");

            var httpContext = _httpContextAccessor.HttpContext;
            
            // Authorization logic - the cross-cutting concern
            // Check if user is admin (checking header for demo purposes)
            // In production, you would check JWT claims, session, etc.
            var isAdmin = httpContext?.Request.Headers.TryGetValue("X-User-Role", out var role) == true 
                          && role.ToString().Equals("Admin", StringComparison.OrdinalIgnoreCase);

            if (!isAdmin)
            {
                Console.WriteLine($"[AOP Interceptor] Access denied! User is not an admin.");
                throw new UnauthorizedAccessException("Access denied. Admin role required.");
            }

            Console.WriteLine($"[AOP Interceptor] Access granted! User is an admin.");
            // ═════ END OF BEFORE ADVICE ═════
        }

        // Proceed with the actual method execution (the join point)
        invocation.Proceed();
        
        // ═════ AFTER ADVICE could be added here if needed ═════
        // For example: logging method completion, cleanup, etc.
    }
}
