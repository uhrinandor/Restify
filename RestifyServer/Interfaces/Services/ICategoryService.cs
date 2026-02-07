using RestifyServer.Dto;
using RestifyServer.TypeContracts;

namespace RestifyServer.Interfaces.Services;

public interface ICategoryService : ICrudService<Category, CreateCategory, UpdateCategory, FindCategory>
{

}
