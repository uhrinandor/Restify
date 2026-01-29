namespace RestifyServer.Configuration;

public static class AppConfiguration
{
    public static string GetSeqUrl(ConfigurationManager Config)
    {
        return Config.GetValue<string>("Serilog:WriteTo:1:Args:serverUrl") ?? throw new Exception("Seq log url was not found...");
    }
}
