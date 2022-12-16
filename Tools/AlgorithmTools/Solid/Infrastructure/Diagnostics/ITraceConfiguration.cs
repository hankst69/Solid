//----------------------------------------------------------------------------------
// <copyright file="ITraceConfiguration.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

namespace Solid.Infrastructure.Diagnostics
{
    public interface ITraceConfiguration
    {
        void ConfigureFromCommandlineArgs(string[] commandLineArgs);

        //void SetTraceLevel(ERROR|WARNING|DEBUG|INFO|INOUT);
        //void SetFileTraceLevel();
        //void SetConsoleTraceLevel();

        void StartFileTracer(string fileName = null, string traceFolder = null);
        void StopFileTracer();

        void StartConsoleTracer();
        void StopConsoleTracer();
    }
}
