using Microsoft.EntityFrameworkCore;
using RestifyServer.Data;
using RestifyServer.Models;
using RestifyServer.Repository;

namespace RestifyServer.Repositories;

public class AdminRepository(RestifyContext db) : Repository<Admin>(db);
