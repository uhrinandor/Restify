using RestifyServer.Dto;
using RestifyServer.TypeContracts;

namespace RestifyServer.Interfaces.Services;

public interface IProductService : ICrudService<Product, CreateProduct, UpdateProduct, FindProduct>;
