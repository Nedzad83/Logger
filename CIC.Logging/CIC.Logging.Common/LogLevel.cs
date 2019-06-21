using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIC.Logging.Common
{
    /// <summary>
    /// Enumeration of log levels. All of those levels are matching NLog levels with the same name.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Error level
        /// </summary>
        Error,
        /// <summary>
        /// Warning level
        /// </summary>
        Warn,
        /// <summary>
        /// Information level
        /// </summary>
        Info,
        /// <summary>
        /// Debugging level
        /// </summary>
        Debug
    }
}
