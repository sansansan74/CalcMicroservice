using CalcMicroservice.RabbitMQ;
using CalcMicroservice.Serilog;
using CalcMicroservice.Services;
using CalcMicroservice.Services.Consumers.CalcOperations.BaseMath;
using CalcMicroservice.Services.Consumers.CalcOperations.UserFunction;
using CalcMicroservice.Services.Consumers.Servicing;
using CalcMicroservice.Services.Workers;
using CalcMicroservice.Utils;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Operations;
using RabbitMQ.Client;
using Serilog;
using System;
using System.Collections.Generic;
using System.Reflection;


/*
  * REGISTER RULES:
  *      EXE WITHOUT ALL ARGS - simple all modules on
  *      EXE +MOD1 +MOD2 - only MOD1 and MOD2. Include syntax.
  *      EXE -MOD1 -MOD2 - all without MOD1 & MOD2. Exclude syntax
  *      EXE -MOD1 +MOD2 - error. You can not mix include and exclude syntaxes
*/


namespace CalcMicroservice;


public partial class Program
{
    
    public static void Main(string[] args)
    {
        CreateAndBindLogQueue();
        //Serilog.Debugging.SelfLog.Enable(Console.Error);
        ModuleStartManager.Init(args);
        var host = CreateHostBuilder(args).Build();
        host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                // Bind RabbitMQ settings
                var rabbitMQSettings = new RabbitMQSettings();
                hostContext.Configuration.GetSection("RabbitMQ").Bind(rabbitMQSettings);
                services.AddSingleton(rabbitMQSettings);
                services.AddSingleton<ITreePersistantStorageService, TreePersistantStorageService>();
                services.AddSingleton<QueueSenderService>();
                services.AddSingleton<CalculateExpressionService>();
                services.AddSingleton<CacheService>();

                // Настройка Redis
                services.AddStackExchangeRedisCache(options =>
                {
                    string url = hostContext.Configuration["Redis:url"];
                    options.Configuration = url; // "localhost:6379"
                });

                ConfigureMassTransit(services);

                services.AddHostedService<ConsoleReaderWorker>();
            })
        .UseSerilog((context, services, configuration) =>
            {//.ReadFrom.Services(services)
                configuration.ReadFrom.Configuration(context.Configuration)
                .Enrich.WithProperty("Hostname", Environment.MachineName)
                .Enrich.WithProperty("AppName", Assembly.GetExecutingAssembly().GetName().Name)
                .Enrich.With(new ThreadIdEnricher())
                .Enrich.With(new TaskIdEnricher());
            });

    private static void ConfigureMassTransit(IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            // regisger servicing consumers
            RegisterConsumer<ParserExpressionConsumer>(x, RoleNames.PARSER);

            // Calc have 2 comsumers
            RegisterConsumer<CalculateExpressionConsumer>(x, RoleNames.CALC);
            RegisterConsumer<OperationResultsConsumer>(x, RoleNames.CALC);

            RegisterConsumer<ProcessingResultConsumer>(x, RoleNames.MAIN);

            // register base calculation consumers
            RegisterConsumer<AddOperationConsumer>(x, RoleNames.CALC_ADD);
            RegisterConsumer<MultOperationConsumer>(x, RoleNames.CALC_MULT);
            RegisterConsumer<DivideOperationConsumer>(x, RoleNames.CALC_DIVIDE);
            RegisterConsumer<SubtractOperationConsumer>(x, RoleNames.CALC_SUBTRACT);

            // register user function consumers
            RegisterUserOperationsConsumers(x);

            x.UsingRabbitMq((context, cfg) =>
            {
                var settings = context.GetRequiredService<RabbitMQSettings>();
                cfg.Host(settings.HostName, h =>
                {
                    h.Username(settings.UserName);
                    h.Password(settings.Password);
                });

                cfg.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter("calc", false));

                RegisterServicingOperationsEndpoints(context, cfg);
                RegisterCalcOperationsEndpoints(context, cfg);
            });
        });
    }



    private static void RegisterServicingOperationsEndpoints(IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator cfg)
    {
        RegisterEndPoint<ProcessingResultConsumer>(RoleNames.MAIN, QueueNames.ProcessingResultMessage, cfg, context);

        RegisterEndPoint<ParserExpressionConsumer>(RoleNames.PARSER, QueueNames.ParserExpressionMessage, cfg, context);

        RegisterEndPoint<OperationResultsConsumer>(RoleNames.CALC, QueueNames.ResultOperationMessage, cfg, context);
        RegisterEndPoint<CalculateExpressionConsumer>(RoleNames.CALC, QueueNames.CalcMessage, cfg, context);
    }

    private static void RegisterCalcOperationsEndpoints(IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator cfg)
    {
        RegisterMainCalcOperationEndpoints(context, cfg);
        RegisterUserOperationsEndpoints(context, cfg);
    }

    private static void RegisterMainCalcOperationEndpoints(IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator cfg)
    {
        RegisterEndPoint<AddOperationConsumer>(RoleNames.CALC_ADD, QueueNames.AddOperationMessage, cfg, context);
        RegisterEndPoint<MultOperationConsumer>(RoleNames.CALC_MULT, QueueNames.MultOperationMessage, cfg, context);
        RegisterEndPoint<DivideOperationConsumer>(RoleNames.CALC_DIVIDE, QueueNames.DivideOperationMessage, cfg, context);
        RegisterEndPoint<SubtractOperationConsumer>(RoleNames.CALC_SUBTRACT, QueueNames.SubtractOperationMessage, cfg, context);
    }

    static void CreateAndBindLogQueue()
    {
        // Define the connection factory
        var factory = new ConnectionFactory()
        {   // todo move to configuration
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };

        string exchangeName = "LogExchange";
        string queueName = "logs";

        // Create a connection
        using (var connection = factory.CreateConnection())
        {
            // Create a channel
            using (var channel = connection.CreateModel())
            {
                // Declare an exchange
                channel.ExchangeDeclare(exchange: exchangeName, type: "direct", durable: true);

                // Declare a queue
                channel.QueueDeclare(queue: queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                // Bind the queue to the exchange
                channel.QueueBind(queue: queueName,
                                  exchange: exchangeName,
                                  routingKey: queueName);
            }
        }
    }

}
