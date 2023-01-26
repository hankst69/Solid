//----------------------------------------------------------------------------------
// <copyright file="ILogger.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2017-2019. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;

namespace Solid.Infrastructure.Diagnostics
{
    /// <summary>
    /// ILogger
    /// </summary>
    public interface ILogger
    {
        void Error(string message, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLine = -1, [CallerFilePath] string callerFilePath = "");
        void Error(Exception ex, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLine = -1, [CallerFilePath] string callerFilePath = "");

        void Info(string message, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLine = -1, [CallerFilePath] string callerFilePath = "");

        void Warning(string message, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLine = -1, [CallerFilePath] string callerFilePath = "");

        void Debug(string message, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLine = -1, [CallerFilePath] string callerFilePath = "");
    }
}
