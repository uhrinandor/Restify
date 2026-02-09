using RestifyServer.Dto;
using RestifyServer.TypeContracts;

namespace RestifyServer.Interfaces.Services;

public interface IOrderService : ICrudService<Order, CreateOrder, UpdateOrder, FindOrder>;
