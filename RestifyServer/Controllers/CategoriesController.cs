using Microsoft.AspNetCore.Mvc;
using RestifyServer.Dto;
using RestifyServer.Interfaces.Services;
using RestifyServer.TypeContracts;

namespace RestifyServer.Controllers;


public class CategoriesController(ICategoryService categoryService) : CrudController<Category, CreateCategory, UpdateCategory, FindCategory>(categoryService)
{

}
