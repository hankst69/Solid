//----------------------------------------------------------------------------------
// <copyright file="MultiTracer.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2020-2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Solid.Infrastructure.RuntimeTypeExtensions;

namespace Solid.Infrastructure.Diagnostics.Impl
{
    /// <summary>
    /// MultiTracer
    /// </summary>
    public class MultiTracer : IMultiTracer
    {
        private readonly IList<ITracer> m_Tracers = new List<ITracer>();

        public IMultiTracer AddTracer(ITracer tracer)
        {
            ConsistencyCheck.EnsureArgument(tracer).IsNotNull();
            m_Tracers.Add(tracer);
            return this;
        }

        public IMultiTracer RemoveTracer(ITracer tracer)
        {
            ConsistencyCheck.EnsureArgument(tracer).IsNotNull();
            m_Tracers.Remove(tracer);
            return this;
        }
        public IMultiTracer RemoveAllTracers()
        {
            m_Tracers.Clear();
            return this;
        }

        public MultiTracer()
        {
        }

        public MultiTracer(IList<ITracer> tracers)
        {
            ConsistencyCheck.EnsureArgument(tracers).IsNotNull();//.IsNotEmpty();
            m_Tracers = tracers;
        }

        public string TraceDomain => m_Tracers.FirstOrDefault()?.TraceDomain;
        public string TraceScope => m_Tracers.FirstOrDefault()?.TraceScope;

        public void Dispose()
        {
            m_Tracers.ForEach(x => x.Dispose());
            m_Tracers.Clear(); 
        }

        public ITracer CreateBaseDomainTracer()
        {
            return new MultiTracer(m_Tracers.Select(x => x.CreateBaseDomainTracer()).ToList());
        }

        public ITracer CreateBaseDomainTracer(Type traceDomain)
        {
            return new MultiTracer(m_Tracers.Select(x => x.CreateBaseDomainTracer(traceDomain)).ToList());
        }

        public ITracer CreateSubDomainTracer(string subDomain)
        {
            return new MultiTracer(m_Tracers.Select(x => x.CreateSubDomainTracer(subDomain)).ToList());
        }

        public ITracer CreateScopeTracer(string scopeName)
        {
            return new MultiTracer(m_Tracers.Select(x => x.CreateScopeTracer(scopeName)).ToList());
        }

        public void Error(string message, string callerName, int callerLine, string callerFilePath)
        {
            m_Tracers.ForEach(x => x.Error(message, callerName, callerLine, callerFilePath));
        }

        public void Error(Exception ex, string callerName, int callerLine, string callerFilePath)
        {
            m_Tracers.ForEach(x => x.Error(ex, callerName, callerLine, callerFilePath));
        }

        public void Info(string message, string callerName, int callerLine, string callerFilePath)
        {
            m_Tracers.ForEach(x => x.Info(message, callerName, callerLine, callerFilePath));
        }

        public void Warning(string message, string callerName, int callerLine, string callerFilePath)
        {
            m_Tracers.ForEach(x => x.Warning(message, callerName, callerLine, callerFilePath));
        }

        public void Debug(string message, string callerName, int callerLine, string callerFilePath)
        {
            m_Tracers.ForEach(x => x.Debug(message, callerName, callerLine, callerFilePath));
        }
    }
}
