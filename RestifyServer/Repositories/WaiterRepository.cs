using Microsoft.EntityFrameworkCore;
using RestifyServer.Models;
using RestifyServer.Repository;

namespace RestifyServer.Repositories;

public class WaiterRepository(RestifyContext db) : Repository<Waiter>(db);
