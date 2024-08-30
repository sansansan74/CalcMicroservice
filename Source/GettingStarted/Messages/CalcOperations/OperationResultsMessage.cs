using System;

namespace CalcMicroservice.Messages.Operations
{
    public class OperationResultsMessage
    {
        public string TraceId { get; set; }
        public string OperationId { get; set; }
        public double Value { get; set; }
        public string OperationName { get; set; }
        public string Arguments { get; set; }
        public string Error { get; set; }
        public string ErrorSource { get; set; }
        public string Expression { get; set; }
        public string MicroserviceName { get; set; }
    }
}
