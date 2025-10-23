using System.Reflection;
using AopExample.Attributes;
using Castle.DynamicProxy;

namespace AopExample.Interceptors;

/// <summary>
/// Interceptor that checks if user has admin role before executing methods marked with RequireAdminAttribute
/// </summary>
public class AdminRoleInterceptor : IInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AdminRoleInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Intercept(IInvocation invocation)
    {
        // Check if the method has RequireAdminAttribute
        var methodInfo = invocation.MethodInvocationTarget ?? invocation.Method;
        var hasRequireAdminAttribute = methodInfo.GetCustomAttribute<RequireAdminAttribute>() != null ||
                                       invocation.TargetType?.GetCustomAttribute<RequireAdminAttribute>() != null;

        if (hasRequireAdminAttribute)
        {
            Console.WriteLine($"[AOP Interceptor] Checking admin role for method: {methodInfo.Name}");

            var httpContext = _httpContextAccessor.HttpContext;
            
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
        }

        // Proceed with the method execution
        invocation.Proceed();
    }
}
