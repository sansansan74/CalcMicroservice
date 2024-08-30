using System;
using System.Collections.Generic;

namespace CalcMicroservice.Messages.Operations
{
    public class AddOperationMessage
    {
        public string TraceId { get; set; }
        /// <summary>
        /// All operations in expression tree have OperationId
        /// All results of calculation operation put to one queue
        /// When we see the OperationResultMessage, we want know, 
        /// for what operation this result
        /// </summary>
        public string OperationId { get; set; }
        public List<double> Arguments { get; set; }
    }
}
