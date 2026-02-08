using RestifyServer.Data;
using RestifyServer.Models;
using RestifyServer.Repository;

namespace RestifyServer.Repositories;

public class TableRepository(RestifyContext db) : Repository<Table>(db);
