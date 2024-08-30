using System;

namespace CalcMicroservice.Services.Consumers.Exceptions
{
    public class CalcMicroserviceException : Exception
    {
        public string MicroserviceNames {  get; set; }
        public CalcMicroserviceException() { }
        public CalcMicroserviceException(string microserviceName, string message) : base(message) {
            MicroserviceNames = microserviceName;
        }
        public CalcMicroserviceException(string microserviceName, string message, Exception inner) : base(message, inner) {
            MicroserviceNames = microserviceName;
        }
    }
}