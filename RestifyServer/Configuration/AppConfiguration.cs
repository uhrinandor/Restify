using Microsoft.EntityFrameworkCore;
using RestifyServer.Repository;
using Serilog;

namespace RestifyServer.Configuration;

public static class AppConfiguration
{
    public static string GetSeqUrl(ConfigurationManager Config)
    {
        return Config.GetValue<string>("Serilog:WriteTo:1:Args:serverUrl") ?? throw new Exception("Seq log url was not found...");
    }

    public static List<string> GetCorsOrigins(ConfigurationManager Config)
    {
        var origins = Config.GetSection("CorsSettings:AllowedOrigins").Get<List<string>>();
        if (origins == null || origins?.Count == 0) return [];

        return origins!;
    }

}
