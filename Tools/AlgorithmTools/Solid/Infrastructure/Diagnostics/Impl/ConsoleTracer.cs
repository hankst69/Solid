//----------------------------------------------------------------------------------
// <copyright file="ConsoleTracer.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2020-2022. All Rights Reserved. Confidential.
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

        public void Dispose()
        {
            DisposeTraceEnvironment();
        }


        protected override void WriteTrace(string level, string message)
        {
            var levelPadded = level.PadRight(9);

            Console.WriteLine("{0} {1}/{2} #** {3} {4} {5} -> {6}<-",
                DateTime.Now.ToString("HH:mm:ss.ffffff"),
                m_ProcessId,
                m_TreadId,
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

        public ITracer CreateSubDomainTracer(string subDomain)
        {
            ConsistencyCheck.EnsureArgument(subDomain).IsNotNull();
            var traceDomain = string.IsNullOrEmpty(TraceDomain) ? subDomain : string.Concat(TraceDomain, "+", subDomain);
            return new ConsoleTracer(traceDomain, string.Empty);
        }

        public ITracer CreateScopeTracer(string scopeName)
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

            m_CreationTime = DateTime.Now;
            m_ProcessId = Process.GetCurrentProcess().Id;
            //m_TreadId = Thread.CurrentThread.ManagedThreadId;

#pragma warning disable 618
            m_TreadId = AppDomain.GetCurrentThreadId();
#pragma warning restore 618

            // entering trace
            Console.WriteLine("{0} {1}/{2} #*[ entering  {3} {4}",
                m_CreationTime.ToString("HH:mm:ss.ffffff"),
                m_ProcessId,
                m_TreadId,
                TraceDomain,
                TraceScope
                );
        }

        private void DisposeTraceEnvironment()
        {
            if (m_IsDisposed)
            {
                return;
            }

            m_IsDisposed = true;

            var now = DateTime.Now;
            var timeSpan = now - m_CreationTime;
            var spentTime = timeSpan.TotalMilliseconds > 9 ?
                string.Format("{0} ms", System.Math.Round(timeSpan.TotalMilliseconds)) :
                string.Format("{0} us", System.Math.Round(1000d * timeSpan.TotalMilliseconds));

            // leaving trace
            Console.WriteLine("{0} {1}/{2} #*] leaving   {3} {4} -> duration={5}",
                now.ToString("HH:mm:ss.ffffff"),
                m_ProcessId,
                m_TreadId,
                TraceDomain,
                TraceScope,
                spentTime
                );
        }

        private bool m_IsDisposed;
        //private bool m_KeepStreamWriterAlive;
        private DateTime m_CreationTime;
        private int m_TreadId;
        private int m_ProcessId;
    }
}
