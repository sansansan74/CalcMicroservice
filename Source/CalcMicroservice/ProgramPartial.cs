using CalcMicroservice.Services.Consumers.CalcOperations.UserFunction;
using CalcMicroservice.Utils;
using MassTransit;
using Operations;
using System;

namespace CalcMicroservice
{
    /*
     * REGISTER RULES:
     *      EXE WITHOUT ALL ARGS - simple all modules on
     *      EXE +MOD1 +MOD2 - only MOD1 and MOD2. Include syntax.
     *      EXE -MOD1 -MOD2 - all without MOD1 & MOD2. Exclude syntax
     *      EXE -MOD1 +MOD2 - error. You can not mix include and exclude syntaxes
     * 
     Модуль
        обслуживающия
            calc
            parser
        операция
            pi
            avg
            plus
     

        class Consumer
        queueName
        RoleName
        OperationName
        register consumer
        register endpoint


        function setup table
        setup in dbTable
            queueName
            operationName
            active(true/false)

        Для отправки CALC Должен знать только имя очереди из настоек
        Получает CALC через общую очередь
     */


    public partial class Program
    {
        private static void RegisterUserOperationsEndpoints(IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator cfg)
        {
            RegisterEndPoint<AbsOperationConsumer>(RoleNames.CALC_ABS, QueueNames.AbsOperationMessage, cfg, context);
            RegisterEndPoint<PiOperationConsumer>(RoleNames.CALC_PI, QueueNames.PiOperationMessage, cfg, context);
            RegisterEndPoint<AvgOperationConsumer>(RoleNames.CALC_AVG, QueueNames.AvgOperationMessage, cfg, context);
        }
 
        private static void RegisterUserOperationsConsumers(IBusRegistrationConfigurator x)
        {
            RegisterConsumer<AbsOperationConsumer>(x, RoleNames.CALC_ABS);
            RegisterConsumer<PiOperationConsumer>(x, RoleNames.CALC_PI);
            RegisterConsumer<AvgOperationConsumer>(x, RoleNames.CALC_AVG);
        }

        public static void RegisterEndPoint<T>(string roleName, string queueName, IRabbitMqBusFactoryConfigurator cfg, IBusRegistrationContext context)
            where T : class, IConsumer
        {
            if (ModuleStartManager.ContainsModule(roleName))
                cfg.ReceiveEndpoint(queueName, e => e.ConfigureConsumer<T>(context));
        }

        public static void RegisterConsumer<T>(IBusRegistrationConfigurator x, string roleName)
            where T : class, IConsumer
        {
            if (ModuleStartManager.ContainsModule(roleName))
                x.AddConsumer<T>();
        }
       

    }

}

