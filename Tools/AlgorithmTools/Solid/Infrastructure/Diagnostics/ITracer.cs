//----------------------------------------------------------------------------------
// <copyright file="ITracer.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2017-2023. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;

namespace Solid.Infrastructure.Diagnostics
{
    /// <summary>
    /// TraceLevel
    /// </summary>
    public enum TraceLevel
    {
        OFF = 0,
        Error = 1, //2 ^ 0,
        Warning = 2, //2 ^ 1,
        Debug = 4, //2 ^ 2,
        Info = 8, //2 ^ 3,
        InOut = 16, //2 ^ 4
    }

    /// <summary>
    /// ITracer
    /// </summary>
    public interface ITracer : ILogger, ITracerCreator, ITracerInfo, IDisposable
    {
        TraceLevel TraceLevel { get; set; } 
    }

    /// <summary>
    /// ITracerCreator
    /// </summary>
    public interface ITracerCreator
    {
        ITracer CreateBaseDomainTracer();
        ITracer CreateBaseDomainTracer(Type traceDomain);
        ITracer CreateSubDomainTracer(string subDomain);

        ITracer CreateScopeTracer([CallerMemberName] string scopeName = "");
    }

    /// <summary>
    /// ITracerInfo
    /// </summary>
    public interface ITracerInfo
    {
        // TraceDomain and TraceScope info is not really necessary for outside world (just for information)
        string TraceDomain { get; }
        string TraceScope { get; }
    }
}
