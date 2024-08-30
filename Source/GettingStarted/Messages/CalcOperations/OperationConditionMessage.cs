using System.Collections.Generic;

namespace CalcMicroservice.Messages.Operations
{
    public class OperationConditionMessage
    {
        public string TraceId { get; set; }
        /// <summary>
        /// All operations in expression tree have OperationId
        /// All results of calculation operation put to one queue
        /// When we see the OperationResultMessage, we want know, 
        /// for what operation this result
        /// </summary>
        public string OperationId { get; set; }
        /// <summary>
        /// Operation name, like add, mult or functionName (abs, sin)
        /// </summary>
        public string OperationName { get; set; }

        /// <summary>
        /// List of arguments in JSon. Like [1.0, 2.0]. Null, if no arguments
        /// </summary>
        public string Arguments { get; set; }

        /// <summary>
        /// All calculation expression, not only current operation
        /// </summary>
        public string Expression { get; set; }

    }
}
