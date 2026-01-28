using RestifyServer.Interfaces;
using RestifyServer.Models;
using RestifyServer.Repositories;

namespace RestifyServer;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IRepository<Admin>, AdminRepository>();
        services.AddScoped<IRepository<Waiter>, WaiterRepository>();
        services.AddScoped<IRepository<Category>, CategoryRepository>();
        services.AddScoped<IRepository<Product>, ProductRepository>();
        services.AddScoped<IRepository<Invoice>, InvoiceRepository>();
        services.AddScoped<IRepository<Table>, TableRepository>();
        services.AddScoped<IRepository<Order>, OrderRepository>();

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        
        return services;
    }
}
