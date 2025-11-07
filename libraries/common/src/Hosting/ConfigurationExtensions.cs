using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Flashcards.Common.Hosting;

public static class ConfigurationExtensions
{
    public static IConfigurationManager AddCommonConfig(this IConfigurationManager config, IHostEnvironment environment)
    {
        var env = environment.EnvironmentName switch
        {
            "Development" => "dev",
            "Staging" => "stg",
            "Production" => "prd",
            _ => throw new Exception("Unknown environment name: " + environment.EnvironmentName)
        };
        config.AddIniFile("appsettings.ini", false);
        config.AddIniFile($"appsettings.{env}.ini", true);
        return config;
    }
}