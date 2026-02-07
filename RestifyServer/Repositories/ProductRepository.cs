using RestifyServer.Models;
using RestifyServer.Repository;

namespace RestifyServer.Repositories;

public class ProductRepository(RestifyContext db) : Repository<Product>(db);
