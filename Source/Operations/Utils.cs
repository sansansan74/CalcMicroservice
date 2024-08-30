namespace Operations
{
    public class Utils
    {
        public static string GetRabbitQueueUri(string queueName) => $"rabbitmq://localhost/{queueName}";

        static Dictionary<string, string> QueueByOperation = new()
        {
            { OperationNames.Add, QueueNames.AddOperationMessage },
            { OperationNames.Mult, QueueNames.MultOperationMessage },
            { OperationNames.Divide, QueueNames.DivideOperationMessage},
            { OperationNames.Subtract, QueueNames.SubtractOperationMessage},
            { OperationNames.Abs, QueueNames.AbsOperationMessage},
            { OperationNames.Pi, QueueNames.PiOperationMessage},
            { OperationNames.Avg, QueueNames.AvgOperationMessage},
        };
        public static string GetQueueByOperation(string operation) => QueueByOperation[operation];
    }
}
