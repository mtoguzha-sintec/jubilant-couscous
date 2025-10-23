namespace AopExample.Attributes;

/// <summary>
/// Attribute to mark methods that require admin role.
/// This will be used by the interceptor to check authorization.
/// 
/// ════════════════════════════════════════════════════════════════
/// AOP CONCEPT #4: POINTCUT (Marker/Annotation)
/// ════════════════════════════════════════════════════════════════
/// This attribute serves as a marker that defines WHERE the aspect should be applied.
/// It acts as a pointcut annotation that identifies join points (method executions)
/// that should be intercepted by the AdminRoleInterceptor aspect.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class RequireAdminAttribute : Attribute
{
}
