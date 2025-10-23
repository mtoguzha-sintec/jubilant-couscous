using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using AopExample.Interceptors;
using AopExample.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers().AddControllersAsServices(); // Important: AddControllersAsServices allows Autofac to resolve controllers
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "AOP Example API", 
        Version = "v1",
        Description = "API demonstrating Aspect-Oriented Programming with Autofac Interceptors"
    });

    // Add a global header parameter for X-User-Role
    c.AddSecurityDefinition("X-User-Role", new OpenApiSecurityScheme
    {
        Description = "Enter 'Admin' to test admin-protected endpoints",
        Name = "X-User-Role",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "X-User-Role"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "X-User-Role"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddHttpContextAccessor(); // Required for interceptor to access HttpContext

// ════════════════════════════════════════════════════════════════
// AOP CONCEPT #5: WEAVING (Runtime Weaving with Castle DynamicProxy)
// ════════════════════════════════════════════════════════════════
// Configure Autofac with Castle DynamicProxy for runtime weaving
// WEAVING is the process of applying aspects to target code.
// This happens at RUNTIME - dynamic proxies are created that wrap classes
// and intercept method calls to apply the aspect logic.
// ════════════════════════════════════════════════════════════════
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    // Register the aspect (AdminRoleInterceptor) in the IoC container
    containerBuilder.RegisterType<AdminRoleInterceptor>()
        .AsSelf()
        .InstancePerLifetimeScope();

    var assembly = typeof(Program).Assembly;
    
    // ═════ WEAVING for Services (Interface-based proxying) ═════
    // AOP CONCEPT #4: POINTCUT (Expression Matching)
    // This Where clause is a POINTCUT EXPRESSION that identifies which types
    // should have the aspect applied (types with [RequireAdmin] attribute).
    containerBuilder.RegisterAssemblyTypes(assembly)
        .Where(t => !t.IsAbstract && 
                    !typeof(ControllerBase).IsAssignableFrom(t) && // Exclude controllers here
                    // POINTCUT: Match types/methods with RequireAdminAttribute
                    (t.GetMethods().Any(m => m.GetCustomAttributes(typeof(AopExample.Attributes.RequireAdminAttribute), true).Any()) ||
                     t.GetCustomAttributes(typeof(AopExample.Attributes.RequireAdminAttribute), true).Any()))
        .AsImplementedInterfaces()
        .EnableInterfaceInterceptors()  // ← WEAVING: Create interface proxies at runtime
        .InterceptedBy(typeof(AdminRoleInterceptor))  // ← Apply the aspect
        .InstancePerLifetimeScope();

    // ═════ WEAVING for Controllers (Class-based proxying) ═════
    // Controllers need class interceptors because they're concrete classes, not interfaces
    // Castle DynamicProxy creates a subclass that overrides virtual methods
    containerBuilder.RegisterAssemblyTypes(assembly)
        .Where(t => typeof(ControllerBase).IsAssignableFrom(t) &&
                    // POINTCUT: Match controllers/methods with RequireAdminAttribute
                    (t.GetMethods().Any(m => m.GetCustomAttributes(typeof(AopExample.Attributes.RequireAdminAttribute), true).Any()) ||
                     t.GetCustomAttributes(typeof(AopExample.Attributes.RequireAdminAttribute), true).Any()))
        .EnableClassInterceptors()  // ← WEAVING: Create class proxies at runtime
        .InterceptedBy(typeof(AdminRoleInterceptor))  // ← Apply the aspect
        .InstancePerLifetimeScope()
        .PropertiesAutowired();
});
// ════════════════════════════════════════════════════════════════
// End of WEAVING configuration
// At runtime, when methods are called, the proxy intercepts them
// and delegates to the AdminRoleInterceptor's Intercept() method
// ════════════════════════════════════════════════════════════════

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AOP Example API v1");
        c.DocumentTitle = "AOP Example - Swagger UI";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("===========================================");
Console.WriteLine("AOP Example with Autofac Interceptors");
Console.WriteLine("===========================================");
Console.WriteLine();
Console.WriteLine("Swagger UI: http://localhost:5054/swagger");
Console.WriteLine();
Console.WriteLine("✅ Auto-Registration Enabled!");
Console.WriteLine("Services AND Controllers with [RequireAdmin] are intercepted!");
Console.WriteLine();
Console.WriteLine("Test the API:");
Console.WriteLine();
Console.WriteLine("📂 Public endpoints (no auth):");
Console.WriteLine("  • GET  /api/user/public");
Console.WriteLine("  • GET  /api/product");
Console.WriteLine("  • GET  /api/report/public");
Console.WriteLine("  • GET  /api/report/download/{id}");
Console.WriteLine();
Console.WriteLine("🔒 Admin-only endpoints (need X-User-Role: Admin):");
Console.WriteLine();
Console.WriteLine("  Method-level [RequireAdmin]:");
Console.WriteLine("    • GET    /api/user/all");
Console.WriteLine("    • DELETE /api/product/{id}");
Console.WriteLine("    • GET    /api/report/detailed");
Console.WriteLine("    • GET    /api/report/financial");
Console.WriteLine();
Console.WriteLine("  Class-level [RequireAdmin] (entire controller):");
Console.WriteLine("    • GET    /api/admin/settings");
Console.WriteLine("    • GET    /api/admin/logs");
Console.WriteLine("    • POST   /api/admin/reset");
Console.WriteLine();
Console.WriteLine("To test in Swagger:");
Console.WriteLine("1. Click 'Authorize' button (lock icon)");
Console.WriteLine("2. Enter 'Admin' in the X-User-Role field");
Console.WriteLine("3. Click 'Authorize' then 'Close'");
Console.WriteLine("4. Try the admin-protected endpoints");
Console.WriteLine("===========================================");
Console.WriteLine();

app.Run();
