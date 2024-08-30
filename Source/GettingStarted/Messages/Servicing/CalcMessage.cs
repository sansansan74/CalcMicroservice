using System;

namespace CalcMicroservice.Messages.Servicing
{
    public class CalcMessage
    {
        public string TraceId { get; set; }
        public string TreeExpression { get; set; }
    }
}
