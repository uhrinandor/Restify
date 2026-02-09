using RestifyServer.Dto;
using RestifyServer.Interfaces.Services;
using RestifyServer.TypeContracts;

namespace RestifyServer.Controllers;

public class OrderController(IOrderService orderService)
    : CrudController<Order, CreateOrder, UpdateOrder, FindOrder>(orderService);
