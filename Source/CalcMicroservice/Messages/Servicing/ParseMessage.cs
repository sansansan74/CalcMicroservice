using System;

namespace CalcMicroservice.Messages.Servicing
{
    public class ParseMessage
    {
        public string TraceId { get; set; }
        public string MathExpression { get; set; }
    }
}
