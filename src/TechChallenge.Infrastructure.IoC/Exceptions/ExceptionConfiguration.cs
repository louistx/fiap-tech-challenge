namespace TechChallenge.Infrastructure.IoC.Exceptions
{
    public class ExceptionConfiguration
    {
        #region Properties

        public string StatusCode { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public string StackTrace { get; set; } = string.Empty;

        #endregion

        #region Constructor

        public ExceptionConfiguration(string statusCode, string message, string details)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
        }

        public ExceptionConfiguration(string statusCode, string message, string details, string stackTrace)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
            StackTrace = stackTrace;
        }

        #endregion
    }
}
