//----------------------------------------------------------------------------------
// File: "MultiTracer.cs"
// Author: Steffen Hanke
// Date: 2020-2023
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
        private readonly IList<ITracer> _tracers = new List<ITracer>();

        public IMultiTracer AddTracer(ITracer tracer)
        {
            ConsistencyCheck.EnsureArgument(tracer).IsNotNull();
            _tracers.Add(tracer);
            return this;
        }

        public IMultiTracer RemoveTracer(ITracer tracer)
        {
            ConsistencyCheck.EnsureArgument(tracer).IsNotNull();
            _tracers.Remove(tracer);
            return this;
        }
        public IMultiTracer RemoveAllTracers()
        {
            _tracers.Clear();
            return this;
        }

        public MultiTracer()
        {
        }

        public MultiTracer(IList<ITracer> tracers)
        {
            ConsistencyCheck.EnsureArgument(tracers).IsNotNull();//.IsNotEmpty();
            _tracers = tracers;
        }

        public string TraceDomain => _tracers.FirstOrDefault()?.TraceDomain;
        public string TraceScope => _tracers.FirstOrDefault()?.TraceScope;

        public TraceLevel TraceLevel 
        { 
            get => _tracers.FirstOrDefault()?.TraceLevel ?? TraceLevel.Off;
            set => _tracers.ForEach(t => t.TraceLevel = value);
        }

        public void Dispose()
        {
            _tracers.ForEach(x => x.Dispose());
            _tracers.Clear(); 
        }

        public ITracer CreateBaseDomainTracer()
        {
            return new MultiTracer(_tracers.Select(x => x.CreateBaseDomainTracer()).ToList());
        }

        public ITracer CreateBaseDomainTracer(Type traceDomain)
        {
            return new MultiTracer(_tracers.Select(x => x.CreateBaseDomainTracer(traceDomain)).ToList());
        }

        public ITracer CreateSubDomainTracer(string subDomain)
        {
            return new MultiTracer(_tracers.Select(x => x.CreateSubDomainTracer(subDomain)).ToList());
        }

        public ITracer CreateScopeTracer(string scopeName)
        {
            return new MultiTracer(_tracers.Select(x => x.CreateScopeTracer(scopeName)).ToList());
        }

        public void Error(string message, string callerName, int callerLine, string callerFilePath)
        {
            _tracers.ForEach(x => x.Error(message, callerName, callerLine, callerFilePath));
        }

        public void Error(Exception ex, string callerName, int callerLine, string callerFilePath)
        {
            _tracers.ForEach(x => x.Error(ex, callerName, callerLine, callerFilePath));
        }

        public void Info(string message, string callerName, int callerLine, string callerFilePath)
        {
            _tracers.ForEach(x => x.Info(message, callerName, callerLine, callerFilePath));
        }

        public void Warning(string message, string callerName, int callerLine, string callerFilePath)
        {
            _tracers.ForEach(x => x.Warning(message, callerName, callerLine, callerFilePath));
        }

        public void Debug(string message, string callerName, int callerLine, string callerFilePath)
        {
            _tracers.ForEach(x => x.Debug(message, callerName, callerLine, callerFilePath));
        }
    }
}
