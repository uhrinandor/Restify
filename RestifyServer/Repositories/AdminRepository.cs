using Microsoft.EntityFrameworkCore;
using RestifyServer.Models;
using RestifyServer.Repository;

namespace RestifyServer.Repositories;

public class AdminRepository(RestifyContext db) : Repository<Admin>(db);
