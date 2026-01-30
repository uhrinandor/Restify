using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using RestifyServer.Interfaces;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.Interfaces.Services;
using RestifyServer.Models;
using RestifyServer.Repositories;
using RestifyServer.Services;

namespace RestifyServer.Configuration;

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
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAdminService, AdminService>();
        return services;
    }

    public static IServiceCollection AddMapper(this IServiceCollection services)
    {
        var mapperConfig = new MapperConfiguration(cfg => MapperConfigFactory.CreateMapperConfiguration(cfg), NullLoggerFactory.Instance);
        var mapper = mapperConfig.CreateMapper();

        services.AddSingleton<IMapper>(mapper);
        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }

    public static IServiceCollection AddUtils(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher<Models.Admin>, PasswordHasher<Models.Admin>>();
        services.AddScoped<IPasswordHasher<Models.Waiter>, PasswordHasher<Models.Waiter>>();

        return services;
    }
}
