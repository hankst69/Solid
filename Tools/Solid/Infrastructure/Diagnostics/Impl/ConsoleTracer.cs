//----------------------------------------------------------------------------------
// <copyright file="ConsoleTracer.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2020-2023. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;

namespace Solid.Infrastructure.Diagnostics.Impl
{
    /// <summary>
    /// ConsoleTracer
    /// </summary>
    public class ConsoleTracer : BaseTracer, IConsoleTracer
    {
        public ConsoleTracer()
        {
            CreateTraceEnvironment(ReadTraceDomainFromCallStack(), string.Empty);
        }

        private ConsoleTracer(string traceDomain, string traceScope)
        {
            ConsistencyCheck.EnsureArgument(traceDomain).IsNotNull();
            ConsistencyCheck.EnsureArgument(traceScope).IsNotNull();
            CreateTraceEnvironment(traceDomain, traceScope);
        }


        protected override void WriteTraceEntry(string message)
        {
            Console.Out.WriteLine(message);
            Console.Out.Flush();
        }


        #region ITracerCreator
        protected override ITracer CreateBaseDomainTracer(string traceDomainName)
        {
            ConsistencyCheck.EnsureArgument(traceDomainName).IsNotNullOrEmpty();
            return TraceDomain.Equals(traceDomainName) ? this : new ConsoleTracer(traceDomainName, string.Empty)
            {
                TraceLevel = TraceLevel,
                TraceScope = "class"
            }.WriteEnterTrace();
        }

        public override ITracer CreateSubDomainTracer(string subDomain)
        {
            ConsistencyCheck.EnsureArgument(subDomain).IsNotNull();
            var traceDomain = string.IsNullOrEmpty(TraceDomain) ? subDomain : string.Concat(TraceDomain, "+", subDomain);
            return new ConsoleTracer(traceDomain, string.Empty)
            {
                TraceLevel = TraceLevel,
                TraceScope = "class"
            }.WriteEnterTrace();
        }

        public override ITracer CreateScopeTracer(string scopeName)
        {
            return new ConsoleTracer(TraceDomain, scopeName)
            {
                TraceLevel = TraceLevel
            }.WriteEnterTrace();
        }
        #endregion
    }
}
