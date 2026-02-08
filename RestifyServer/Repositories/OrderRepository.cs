using RestifyServer.Data;
using RestifyServer.Models;
using RestifyServer.Repository;

namespace RestifyServer.Repositories;

public class OrderRepository(RestifyContext db) : Repository<Order>(db);
