namespace Operations
{
    /// <summary>
    /// This class contains name of queues.
    /// In future this class must read queue names from DB
    /// This would be useful for new pluggable user operations, like abs, sqrt and others
    /// </summary>
    public static class QueueNames
    {
        // user function queues
        public const string AbsOperationMessage = "operation_abs_message";
        public const string PiOperationMessage = "operation_pi_message";
        public const string AvgOperationMessage = "operation_avg_message";

        // base calculation queues +, -, *, /
        public const string AddOperationMessage = "operation_add_message";
        public const string MultOperationMessage = "operation_mult_message";
        public const string DivideOperationMessage = "operation_divide_message";
        public const string SubtractOperationMessage = "operation_subtract_message";

        // servicing queus
        public const string CalcMessage = "calc_message";
        public const string ResultOperationMessage = "operation_result_message";

        public const string ParserExpressionMessage = "parser_expression_message";

        public const string ProcessingResultMessage = "processing_result_message";
        
    }
}
