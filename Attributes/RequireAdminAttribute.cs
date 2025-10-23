namespace AopExample.Attributes;

/// <summary>
/// Attribute to mark methods that require admin role.
/// This will be used by the interceptor to check authorization.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class RequireAdminAttribute : Attribute
{
}
