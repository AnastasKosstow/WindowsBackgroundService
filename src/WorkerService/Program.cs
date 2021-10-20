using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WorkerService.Configuration;
using Serilog;

namespace WorkerService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json", false, true);

            if (environment != null)
            {
                configurationBuilder.AddJsonFile($"appsettings.{environment}.json", false, true);
            }

            var configuration = configurationBuilder.Build();

            var workerServiceConfiguration = new WorkerServiceConfiguration();
            configuration.Bind(workerServiceConfiguration);

            await CreateHostBuilder(args, workerServiceConfiguration)
                .Build()
                .RunAsync();
        }

        // To use the background service as windows service
        // Install package -> Microsoft.Extensions.Hosting.WindowsServices

        // To start the service:
        // PowerShell -> New-Service -Name {SERVICE NAME} -BinaryPathName "{EXE FILE PATH}" -Description "{DESCRIPTION}" -DisplayName "{DISPLAY NAME}" -StartupType Automatic

        public static IHostBuilder CreateHostBuilder(string[] args, WorkerServiceConfiguration workerServiceConfiguration) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService() // Add this extension to use the app as windows service
                .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom
                        .Configuration(hostContext.Configuration)
                        .CreateLogger();
                    services.AddSingleton(workerServiceConfiguration);
                    services.AddHttpClient();
                    services.AddHostedService<Worker>();
                });
    }
}
