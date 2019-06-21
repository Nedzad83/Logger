using System;
using System.Collections.Generic;

namespace CIC.Logging.Common
{
    /// <summary>
    /// Log entry
    /// </summary>
    public class LogEntry
    {
        public Exception Exception { get; set; }
        public string Message { get; set; }
        public Dictionary<string, object> Attributes { get; set; }
        public LogLevel Level { get; set; }
    }
}