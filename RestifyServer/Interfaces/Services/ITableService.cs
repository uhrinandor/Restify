using RestifyServer.Dto;
using RestifyServer.TypeContracts;

namespace RestifyServer.Interfaces.Services;

public interface ITableService : ICrudService<Table, CreateTable, UpdateTable, FindTable>;
