using LogWriter.Config;
using LogWriter.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RabbitMQ.Client;

namespace LogWriter;

internal class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        })
        .ConfigureServices((hostContext, services) =>
        {
            var configuration = hostContext.Configuration;
            services.Configure<AppSettings>(configuration);

            services.AddSingleton<IMongoClient, MongoClient>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<AppSettings>>().Value.MongoDB;
                return new MongoClient(settings.ConnectionString);
            });

            services.AddSingleton(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<AppSettings>>().Value.RabbitMQ;
                var factory = new ConnectionFactory
                {
                    HostName = settings.HostName,
                    UserName = settings.UserName,
                    Password = settings.Password,
                    DispatchConsumersAsync = true,
                };
                return factory.CreateConnection();
            });

            services.AddSingleton<MongoDbSaverService>();
            services.AddHostedService<LogProcessorService>();
        });

        var host = builder.Build();

        var cts = new CancellationTokenSource();

        Task.Run(() =>
        {
            Console.WriteLine("Press ENTER to exit...");
            Console.ReadLine();
            cts.Cancel();
        });

        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            eventArgs.Cancel = true;
            cts.Cancel();
        };

        await host.RunAsync(cts.Token);
    }
}
