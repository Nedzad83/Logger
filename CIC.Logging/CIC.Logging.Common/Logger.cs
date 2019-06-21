using System;
using System.Collections.Generic;
using NLog.Fluent;
using System.Threading;

namespace CIC.Logging.Common
{
    /// <summary>
    /// Logger
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Logger 
        /// </summary>
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 
        /// </summary>
        private static long _counter;

        /// <summary>
        /// Save log
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static bool Log(string message, LogLevel level)
        {
            return Log(message, level, null);
        }

        /// <summary>
        /// Save log
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static bool Log(string message, LogLevel level, Dictionary<string, object> attributes)
        {
            return Log(message, null, level, attributes);
        }

        /// <summary>
        /// Save log
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static bool Log(string message, Exception exc, LogLevel level)
        {
            return Log(message, exc, level, null);
        }

        /// <summary>
        /// Save log
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static bool Log(string message, Exception exc, LogLevel level, Dictionary<string, object> attributes)
        {
            bool ret = false;
            try
            {
                if (attributes == null)
                    attributes = new Dictionary<string, object>();

                LogEntry logEntry = new LogEntry()
                {
                    Exception = exc,
                    Message = message,
                    Attributes = attributes,
                    Level = level
                };

                ret = Log(logEntry);
            }
            catch
            { }

            return ret;
        }

        /// <summary>
        /// Method for storing LogEntry objects.
        /// </summary>
        /// <param name="logEntry">LogEntry object</param>
        private static bool Log(LogEntry logEntry)
        {
            bool ret = false;

            try
            {
                switch (logEntry.Level)
                {
                    case LogLevel.Debug:
                        if (!_logger.IsDebugEnabled) return false;
                        _logger.Debug().Exception(logEntry.Exception).Message(logEntry.Message).Properties(GetLoggingAttributes(logEntry)).Write();
                        break;
                    case LogLevel.Info:
                        if (!_logger.IsInfoEnabled) return false;
                        _logger.Info().Exception(logEntry.Exception).Message(logEntry.Message).Properties(GetLoggingAttributes(logEntry)).Write();
                        break;
                    case LogLevel.Warn:
                        if (!_logger.IsWarnEnabled) return false;
                        _logger.Warn().Exception(logEntry.Exception).Message(logEntry.Message).Properties(GetLoggingAttributes(logEntry)).Write();
                        break;
                    case LogLevel.Error:
                        if (!_logger.IsErrorEnabled) return false;
                        _logger.Error().Exception(logEntry.Exception).Message(logEntry.Message).Properties(GetLoggingAttributes(logEntry)).Write();
                        break;
                }

                ret = true;
            }
            catch (Exception exc)
            {
                // We really cannot do anything here
            }

            return ret;
        }

        private static Dictionary<string, object> GetLoggingAttributes(LogEntry logEntry)
        {
            Dictionary<string, object> attributes = new Dictionary<string, object>();

            long counter = Interlocked.Increment(ref _counter);

            foreach (var att in LoggingContext.AppDomainLevelAttributes)
                attributes[att.Key] = att.Value;

            foreach (var att in LoggingContext.ThreadLevelAttributes)
                attributes[att.Key] = att.Value;

            foreach (var att in logEntry.Attributes)
                attributes[att.Key] = att.Value;

            attributes["logcounter"] = counter;

            return attributes;
        }
    }
}
