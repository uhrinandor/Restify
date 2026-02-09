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
    extension(IServiceCollection services)
    {
        public IServiceCollection AddRepositories()
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

        public IServiceCollection AddEntityServices()
        {
            services.AddScoped<IEntityService<Admin>, EntityService<Admin>>();
            services.AddScoped<IEntityService<Waiter>, EntityService<Waiter>>();
            services.AddScoped<IEntityService<Category>, EntityService<Category>>();
            services.AddScoped<IEntityService<Product>, EntityService<Product>>();
            services.AddScoped<IEntityService<Invoice>, EntityService<Invoice>>();
            services.AddScoped<IEntityService<Order>, EntityService<Order>>();
            services.AddScoped<IEntityService<Table>, EntityService<Table>>();

            return services;
        }

        public IServiceCollection AddServices()
        {
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IWaiterService, WaiterService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ITableService, TableService>();
            return services;
        }

        public IServiceCollection AddMapper()
        {
            var mapperConfig = new MapperConfiguration(cfg => MapperConfigFactory.CreateMapperConfiguration(cfg), NullLoggerFactory.Instance);
            var mapper = mapperConfig.CreateMapper();

            services.AddSingleton<IMapper>(mapper);
            return services;
        }

        public IServiceCollection AddSwagger()
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }

        public IServiceCollection AddUtils()
        {
            services.AddScoped<IPasswordHasher<Models.Admin>, PasswordHasher<Models.Admin>>();
            services.AddScoped<IPasswordHasher<Models.Waiter>, PasswordHasher<Models.Waiter>>();

            return services;
        }
    }
}
