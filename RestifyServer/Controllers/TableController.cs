using RestifyServer.Dto;
using RestifyServer.Interfaces.Services;
using RestifyServer.TypeContracts;

namespace RestifyServer.Controllers;

public class TableController(ITableService tableService)
    : CrudController<Table, CreateTable, UpdateTable, FindTable>(tableService)
{

}
