using System;

namespace CalcMicroservice.Messages.Servicing
{
    /*
        This message use to return result to WebApi.
        This is finish result: Ok or Error
     */
    public class ProcessingResultMessage
    {
        public string TraceId { get; set; }
        public double Value { get; set; }
        public string Error { get; set; }       // if not null, then error occured
        public string ErrorSource { get; set; } // what microservice send error
    }
}
