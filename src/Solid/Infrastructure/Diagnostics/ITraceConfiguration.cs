//----------------------------------------------------------------------------------
// File: "ITraceConfiguration.cs"
// Author: Steffen Hanke
// Date: 2022
//----------------------------------------------------------------------------------

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
