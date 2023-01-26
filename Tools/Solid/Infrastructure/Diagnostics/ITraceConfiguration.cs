//----------------------------------------------------------------------------------
// <copyright file="ITraceConfiguration.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System.Diagnostics.Tracing;

namespace Solid.Infrastructure.Diagnostics
{
    /// <summary>
    /// TraceTarget
    /// </summary>
    public enum TraceTarget
    {
        Off = 0,
        File = 1,    // 2^0
        Console = 2  // 2^1
    }

    public interface ITraceConfiguration
    {
        public void ConfigureFromEnvironment();

        string[] ConfigureFromCommandlineArgs(string[] commandLineArgs);

        TraceLevel TraceLevel { get; set; }

        void StartFileTracer(string fileName = null, string traceFolder = null);
        void StopFileTracer();
        TraceLevel FileTraceLevel { get; set; }

        void StartConsoleTracer();
        void StopConsoleTracer();
        TraceLevel ConsoleTraceLevel { get; set; }
    }
}
