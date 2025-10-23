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

// Configure Autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    // Register the interceptor
    containerBuilder.RegisterType<AdminRoleInterceptor>()
        .AsSelf()
        .InstancePerLifetimeScope();

    var assembly = typeof(Program).Assembly;
    
    // Auto-register services with RequireAdmin attribute (interface interception)
    containerBuilder.RegisterAssemblyTypes(assembly)
        .Where(t => !t.IsAbstract && 
                    !typeof(ControllerBase).IsAssignableFrom(t) && // Exclude controllers here
                    (t.GetMethods().Any(m => m.GetCustomAttributes(typeof(AopExample.Attributes.RequireAdminAttribute), true).Any()) ||
                     t.GetCustomAttributes(typeof(AopExample.Attributes.RequireAdminAttribute), true).Any()))
        .AsImplementedInterfaces()
        .EnableInterfaceInterceptors()
        .InterceptedBy(typeof(AdminRoleInterceptor))
        .InstancePerLifetimeScope();

    // Auto-register controllers with RequireAdmin attribute (class interception)
    // Controllers need class interceptors because they're concrete classes, not interfaces
    containerBuilder.RegisterAssemblyTypes(assembly)
        .Where(t => typeof(ControllerBase).IsAssignableFrom(t) &&
                    (t.GetMethods().Any(m => m.GetCustomAttributes(typeof(AopExample.Attributes.RequireAdminAttribute), true).Any()) ||
                     t.GetCustomAttributes(typeof(AopExample.Attributes.RequireAdminAttribute), true).Any()))
        .EnableClassInterceptors()
        .InterceptedBy(typeof(AdminRoleInterceptor))
        .InstancePerLifetimeScope()
        .PropertiesAutowired();
});

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
