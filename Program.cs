using ConsumerApp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


// Default builder to inject user secrets and services
var host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(app =>
            {
                //injecting user secrets
                app.AddUserSecrets("8f282f66-069f-4706-940f-723c2210f397");
                app.AddAzureAppConfiguration("Endpoint=https://appconfig-kafka.azconfig.io;Id=iZE2-lh-s0:pZCsv+hq4Ls8y1LhCsGB;Secret=EuL9V4HBqKoK5tB+zlcXbsXNn2kTAOAnWV2WWR/X8Dw=");
                app.AddJsonFile("appsettings.json");
            })
            .ConfigureLogging((context, config) =>
            config.AddConfiguration(context.Configuration.GetSection("Logging")))
            .ConfigureServices(services =>
            {

                ////injecting database services
                //services.AddSqlDatabaseConnector();

                ////injecting producer services
                //services.AddProducerConnector();

                ////injecting consumer services
                //services.AddSingleton<IConsumerServices, ConsumerServices>();

                ////injecting consumer background services
                //services.AddHostedService<ConsumerBackgroundService>();

                services.AddServices();

                //services.AddLogging(config => config.ClearProviders().AddConsole().SetMinimumLevel(LogLevel.Information));
            });

//building and running the host
host.RunConsoleAsync();

Console.ReadLine();
