# AOP Example with Autofac Interceptors

This project demonstrates **Aspect-Oriented Programming (AOP)** in ASP.NET Core Web API using **Autofac** and **Castle DynamicProxy** interceptors.

## What This Project Does

- Uses AOP to check if a user has **Admin role** before executing certain methods
- Implements authorization check using interceptors (cross-cutting concerns)
- Demonstrates clean separation of business logic from authorization logic

## Project Structure

```
AopExample/
├── Attributes/
│   └── RequireAdminAttribute.cs      # Custom attribute to mark admin-only methods
├── Interceptors/
│   └── AdminRoleInterceptor.cs       # Interceptor that checks admin role
├── Services/
│   └── IUserService.cs                # Service interface and implementation
├── Controllers/
│   └── UserController.cs              # API controller
└── Program.cs                         # Autofac configuration
```

## Key Components

### 1. RequireAdminAttribute
A custom attribute used to mark methods that require admin access.

### 2. AdminRoleInterceptor
- Implements `Castle.DynamicProxy.IInterceptor`
- Checks if method has `[RequireAdmin]` attribute
- Validates user role from HTTP header `X-User-Role`
- Throws `UnauthorizedAccessException` if user is not admin

### 3. UserService
- Contains methods with and without `[RequireAdmin]` attribute
- Methods are intercepted automatically by Autofac

### 4. Autofac Configuration
In `Program.cs`:
```csharp
containerBuilder.RegisterType<AdminRoleInterceptor>()
    .AsSelf()
    .InstancePerLifetimeScope();

containerBuilder.RegisterType<UserService>()
    .As<IUserService>()
    .EnableInterfaceInterceptors()           // Enable interception
    .InterceptedBy(typeof(AdminRoleInterceptor))  // Specify interceptor
    .InstancePerLifetimeScope();
```

## How to Run

1. **Build and run the project:**
   ```bash
   dotnet run
   ```

2. **Navigate to Swagger UI:**
   ```
   https://localhost:<port>/swagger
   ```

## API Endpoints

### 1. GET `/api/user/public` - Public Access
No authorization required.

**Request:**
```bash
curl -X GET https://localhost:7000/api/user/public
```

**Response:**
```json
{
  "success": true,
  "data": "This is public data accessible to everyone"
}
```

### 2. GET `/api/user/all` - Admin Only
Requires `X-User-Role: Admin` header.

**Request WITHOUT admin header (FAILS):**
```bash
curl -X GET https://localhost:7000/api/user/all
```

**Response:**
```json
{
  "success": false,
  "message": "Access denied. Admin role required."
}
```

**Request WITH admin header (SUCCESS):**
```bash
curl -X GET https://localhost:7000/api/user/all \
  -H "X-User-Role: Admin"
```

**Response:**
```json
{
  "success": true,
  "data": "List of all users: [Admin, User1, User2, User3]"
}
```

## Testing with Swagger

1. Open Swagger UI
2. Click on **GET /api/user/all**
3. Click **Try it out**
4. Click **Execute** - Should get 401 Unauthorized
5. Click **Parameters** and add header:
   - Key: `X-User-Role`
   - Value: `Admin`
6. Click **Execute** again - Should get 200 OK

## How AOP Works Here

1. **Request comes in** → Controller calls `_userService.GetAllUsers()`
2. **Autofac intercepts** → Before executing the method, AdminRoleInterceptor runs
3. **Interceptor checks** → Looks for `[RequireAdmin]` attribute
4. **Authorization check** → Validates `X-User-Role` header
5. **Decision**:
   - ✅ If Admin → Method executes normally
   - ❌ If Not Admin → Throws UnauthorizedAccessException

## Benefits of AOP

- ✅ **Separation of Concerns**: Authorization logic is separate from business logic
- ✅ **Reusability**: Same interceptor can be used across multiple services
- ✅ **Maintainability**: Easy to modify authorization logic in one place
- ✅ **Clean Code**: Business methods remain focused on their core functionality

## Technologies Used

- ASP.NET Core 9.0
- Autofac 8.4.0
- Autofac.Extensions.DependencyInjection 10.0.0
- Autofac.Extras.DynamicProxy 7.1.0 (Castle.DynamicProxy)
- Swashbuckle.AspNetCore 9.0.6 (Swagger)

## Production Considerations

In a real-world application, you would:
- Use JWT tokens instead of simple headers
- Check claims from authenticated user
- Integrate with Identity providers
- Use proper authentication middleware
- Add logging to interceptors
- Handle async methods properly
# jubilant-couscous
# jubilant-couscous
