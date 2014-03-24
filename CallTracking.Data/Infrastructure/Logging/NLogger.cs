using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallTracking.Data.Infrastructure.Logging
{
    public interface ILogger
    {
        void LogDebug(string message);
        void LogError(Exception x);
        void LogError(string message);
        void LogFatal(Exception x);
        void LogFatal(string message);
        void LogInfo(string message);
        void LogWarning(string message);
    }
    public class NLogger : ILogger
    {
        Logger _logger;
        public NLogger()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }
        public void LogInfo(string message)
        {
            _logger.Info(message);
        }

        public void LogWarning(string message)
        {
            _logger.Warn(message);
        }

        public void LogDebug(string message)
        {
            _logger.Debug(message);
        }

        public void LogError(string message)
        {
            _logger.Error(message);
        }
        public void LogError(Exception x)
        {
            LogError(BuildExceptionMessage(x));
        }
        public void LogFatal(string message)
        {
            _logger.Fatal(message);
        }
        public void LogFatal(Exception x)
        {
            LogFatal(BuildExceptionMessage(x));
        }
        string BuildExceptionMessage(Exception x)
        {

            Exception logException = x;
            if (x.InnerException != null)
                logException = x.InnerException;

            // Get the error message
            string strErrorMsg = Environment.NewLine + "Message :" + logException.Message;

            // Source of the message
            strErrorMsg += Environment.NewLine + "Source :" + logException.Source;

            // Stack Trace of the error

            strErrorMsg += Environment.NewLine + "Stack Trace :" + logException.StackTrace;

            // Method where the error occurred
            strErrorMsg += Environment.NewLine + "TargetSite :" + logException.TargetSite;
            return strErrorMsg;
        }
    }
}
