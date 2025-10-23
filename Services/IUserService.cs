using AopExample.Attributes;

namespace AopExample.Services;

public interface IUserService
{
    string GetAllUsers();
    string GetPublicData();
}

public class UserService : IUserService
{
    [RequireAdmin]
    public virtual string GetAllUsers()
    {
        return "List of all users: [Admin, User1, User2, User3]";
    }

    public virtual string GetPublicData()
    {
        return "This is public data accessible to everyone";
    }
}
