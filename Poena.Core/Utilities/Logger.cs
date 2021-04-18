using System;
using System.Diagnostics;
using Poena.Core.Common;

namespace Poena.Core.Utilities
{
    public static class Logger {

        /// <summary>
        /// Signleton logger for logging out to different areas based on
        /// context
        /// </summary>
        /// <value>
        /// Gets and sets the logger
        /// </value>
        private static LoggerInstance _logger { get; set; }

        public class LoggerInstance {

            public LogLevel LogLevel { get; set;}

            public void Log(LogLevel logLevel, string nameSpace, string entry) 
            {
                if (this.LogLevel == logLevel) {
                    Debug.WriteLine($"{nameSpace} -- {DateTime.Now.ToString("hh:mm:ss")} -- {entry}");
                }
            }
        }

        public static LoggerInstance GetInstance() {
            _logger = _logger ?? new LoggerInstance();

            return _logger;
        }

    }

}