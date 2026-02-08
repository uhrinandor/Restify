using RestifyServer.Data;
using RestifyServer.Models;
using RestifyServer.Repository;

namespace RestifyServer.Repositories;

public class InvoiceRepository(RestifyContext db) : Repository<Invoice>(db);
