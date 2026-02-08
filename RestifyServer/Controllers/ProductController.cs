using RestifyServer.Dto;
using RestifyServer.Interfaces.Services;
using RestifyServer.TypeContracts;

namespace RestifyServer.Controllers;

public class ProductController(IProductService productService) : CrudController<Product, CreateProduct, UpdateProduct, FindProduct>(productService)
{

}
