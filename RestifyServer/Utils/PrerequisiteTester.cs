using Microsoft.EntityFrameworkCore;
using RestifyServer.Data;
using RestifyServer.Repository;
using Serilog;

namespace RestifyServer.Utils;

public static class PrerequisiteTester
{
    public static void TestDbConnection(this WebApplication application)
    {
        using (var scope = application.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<RestifyContext>();
            try
            {
                if (context.Database.CanConnect())
                {
                    Log.Information("DATABASE CONNECTED: {Host}",
                        context.Database.GetDbConnection().DataSource);
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "DATABASE REJECTED: App tried to connect to {Host}",
                    context.Database.GetDbConnection().DataSource);
                throw new Exception("DATABASE REJECTED: " + ex.Message);
            }
        }
    }
}
