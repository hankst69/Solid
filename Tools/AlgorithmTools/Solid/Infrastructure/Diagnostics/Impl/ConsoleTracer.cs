//----------------------------------------------------------------------------------
// <copyright file="ConsoleTracer.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2020-2023. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;
using System.Diagnostics;

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


        protected override void WriteTrace(string level, string message)
        {
            var levelPadded = level.PadRight(9);

            Console.WriteLine("{0} {1}/{2} #** {3} {4} {5} -> {6}<-",
                DateTime.Now.ToString("HH:mm:ss.ffffff"),
                _processId,
                _threadId,
                levelPadded,
                TraceDomain,
                TraceScope,
                message
                );
        }


        #region ITracerCreator
        protected override ITracer CreateBaseDomainTracer(string traceDomainName)
        {
            ConsistencyCheck.EnsureArgument(traceDomainName).IsNotNullOrEmpty();
            return TraceDomain.Equals(traceDomainName) ? this : new ConsoleTracer(traceDomainName, string.Empty);
        }

        public override ITracer CreateSubDomainTracer(string subDomain)
        {
            ConsistencyCheck.EnsureArgument(subDomain).IsNotNull();
            var traceDomain = string.IsNullOrEmpty(TraceDomain) ? subDomain : string.Concat(TraceDomain, "+", subDomain);
            return new ConsoleTracer(traceDomain, string.Empty);
        }

        public override ITracer CreateScopeTracer(string scopeName)
        {
            return new ConsoleTracer(TraceDomain, scopeName);
        }
        #endregion


        private void CreateTraceEnvironment(string traceDomain, string traceScope)
        {
            ConsistencyCheck.EnsureArgument(traceDomain).IsNotNull();
            ConsistencyCheck.EnsureArgument(traceScope).IsNotNull();

            TraceDomain = traceDomain;
            TraceScope = traceScope;

            _creationTime = DateTime.Now;
            _processId = Process.GetCurrentProcess().Id;
            //_threadId = Thread.CurrentThread.ManagedThreadId;

#pragma warning disable 618
            _threadId = AppDomain.GetCurrentThreadId();
#pragma warning restore 618

            // entering trace
            Console.WriteLine("{0} {1}/{2} #*[ entering  {3} {4}",
                _creationTime.ToString("HH:mm:ss.ffffff"),
                _processId,
                _threadId,
                TraceDomain,
                TraceScope
                );
        }

        protected override void DisposeTraceEnvironment()
        {
            var now = DateTime.Now;
            var timeSpan = now - _creationTime;
            var spentTime = timeSpan.TotalMilliseconds > 9 ?
                string.Format("{0} ms", System.Math.Round(timeSpan.TotalMilliseconds)) :
                string.Format("{0} us", System.Math.Round(1000d * timeSpan.TotalMilliseconds));

            // leaving trace
            Console.WriteLine("{0} {1}/{2} #*] leaving   {3} {4} -> duration={5}",
                now.ToString("HH:mm:ss.ffffff"),
                _processId,
                _threadId,
                TraceDomain,
                TraceScope,
                spentTime
                );
        }

        private DateTime _creationTime;
        private int _threadId;
        private int _processId;
    }
}
