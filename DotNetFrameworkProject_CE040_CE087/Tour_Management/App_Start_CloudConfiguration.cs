using System;
using System.Configuration;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Tour_Management
{
    internal static class CloudConfiguration
    {
        private static readonly Lazy<IConfigurationRoot> ConfigurationRoot = new Lazy<IConfigurationRoot>(BuildConfiguration);

        public static IConfigurationRoot Build()
        {
            return ConfigurationRoot.Value;
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                              ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                              ?? "Production";

            return new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();
        }

        public static string GetConnectionString(string name)
        {
            var configuration = Build();
            var configuredValue = configuration.GetConnectionString(name);
            if (!string.IsNullOrWhiteSpace(configuredValue))
            {
                return configuredValue;
            }

            var legacyValue = ConfigurationManager.ConnectionStrings[name]?.ConnectionString;
            return legacyValue ?? string.Empty;
        }
    }
}