//----------------------------------------------------------------------------------
// File: "NullLogger.cs"
// Author: Steffen Hanke
// Date: 2019
//----------------------------------------------------------------------------------
using System;

namespace Solid.Infrastructure.Diagnostics.Impl
{
    /// <summary>
    /// NullLogger
    /// </summary>
    public class NullLogger : ILogger
    {
        public void Error(string message, string callerName, int callerLine, string callerFilePath)
        { }

        public void Error(string format, object arg1, object arg2, object arg3, object arg4, string callerName, int callerLine, string callerFilePath)
        { }

        public void Error(Exception ex, string callerName, int callerLine, string callerFilePath)
        { }

        public void Error(Exception ex, string message, string callerName, int callerLine, string callerFilePath)
        { }

        public void Error(Exception ex, string format, object arg1, object arg2, object arg3, object arg4, string callerName, int callerLine, string callerFilePath)
        { }

        public void Info(string message, string callerName, int callerLine, string callerFilePath)
        { }

        public void Info(string format, object arg1, object arg2, object arg3, object arg4, string callerName, int callerLine, string callerFilePath)
        { }

        public void Warning(string message, string callerName, int callerLine, string callerFilePath)
        { }

        public void Warning(string format, object arg1, object arg2, object arg3, object arg4, string callerName, int callerLine, string callerFilePath)
        { }

        public void Debug(string message, string callerName, int callerLine, string callerFilePath)
        { }

        public void Debug(string format, object arg1, object arg2, object arg3, object arg4, string callerName, int callerLine, string callerFilePath)
        { }
    }
}
