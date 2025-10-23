using AopExample.Attributes;

namespace AopExample.Services;

public interface IUserService
{
    string GetAllUsers();
    string GetPublicData();
}

public class UserService : IUserService
{
    /// <summary>
    /// ════════════════════════════════════════════════════════════════
    /// AOP CONCEPT #2: JOIN POINT
    /// ════════════════════════════════════════════════════════════════
    /// This method execution is a JOIN POINT - a specific point in program execution
    /// where the aspect (AdminRoleInterceptor) can be applied.
    /// The [RequireAdmin] attribute marks this as a pointcut, causing the interceptor
    /// to check authorization before this method executes.
    /// </summary>
    [RequireAdmin]
    public virtual string GetAllUsers()
    {
        return "List of all users: [Admin, User1, User2, User3]";
    }

    /// <summary>
    /// This method is NOT a join point for the AdminRoleInterceptor
    /// because it lacks the [RequireAdmin] attribute.
    /// It executes normally without interception.
    /// </summary>
    public virtual string GetPublicData()
    {
        return "This is public data accessible to everyone";
    }
}
