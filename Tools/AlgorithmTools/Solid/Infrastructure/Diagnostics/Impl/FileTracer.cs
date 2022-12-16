//----------------------------------------------------------------------------------
// <copyright file="FileTracer.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2019-2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using Solid.Infrastructure.Environment;

using System;
using System.Diagnostics;
using System.IO;

namespace Solid.Infrastructure.Diagnostics.Impl
{
    /// <summary>
    /// FileTracer
    /// </summary>
    public class FileTracer : BaseTracer, IFileTracer
    {
        private IFolderProvider _folderProvider;

        public FileTracer()
        {
            CreateTraceEnvironment(ReadTraceDomainFromCallStack(), string.Empty);
        }

        public FileTracer(string fileName)
        {
            ConsistencyCheck.EnsureArgument(fileName).IsNotNullOrEmpty();
            fileName = _folderProvider.EnsureValidPathName(fileName);
            fileName = _folderProvider.EnsureValidFileName(fileName);
            CreateTraceEnvironment(ReadTraceDomainFromCallStack(), string.Empty, new StreamWriter(fileName));
        }

        public FileTracer(IFolderProvider folderProvider)
        {
            ConsistencyCheck.EnsureArgument(folderProvider).IsNotNull();
            _folderProvider = folderProvider;
            CreateTraceEnvironment(ReadTraceDomainFromCallStack(), string.Empty);
        }

        private FileTracer(string traceDomain, string traceScope, StreamWriter traceStreamWriter)
        {
            //ConsistencyCheck.EnsureArgument(traceStreamWriter).IsNotNull();
            ConsistencyCheck.EnsureArgument(traceDomain).IsNotNull();
            ConsistencyCheck.EnsureArgument(traceScope).IsNotNull();
            CreateTraceEnvironment(traceDomain, traceScope, traceStreamWriter);
        }

        public void Dispose()
        {
            DisposeTraceEnvironment();
        }


        #region ITracerCreator
        protected override ITracer CreateBaseDomainTracer(string traceDomainName)
        {
            ConsistencyCheck.EnsureArgument(traceDomainName).IsNotNullOrEmpty();
            return traceDomainName.Equals(TraceDomain) ? this : new FileTracer(traceDomainName, string.Empty, m_TraceStreamWriter);
        }

        public ITracer CreateSubDomainTracer(string subDomain)
        {
            ConsistencyCheck.EnsureArgument(subDomain).IsNotNull();
            var traceDomain = string.IsNullOrEmpty(TraceDomain) ? subDomain : string.Concat(TraceDomain, "+", subDomain);
            return new FileTracer(traceDomain, string.Empty, m_TraceStreamWriter);
        }

        public ITracer CreateScopeTracer(string scopeName)
        {
            return new FileTracer(TraceDomain, scopeName, m_TraceStreamWriter);
        }
        #endregion


        #region ILogger
        protected override void WriteTrace(string level, string message)
        {
            var levelPadded = level.PadRight(9);

            m_TraceStreamWriter?.WriteLine("{0} {1}/{2} #** {3} {4} {5} -> {6}<-",
                DateTime.Now.ToString("HH:mm:ss.ffffff"),
                m_ProcessId,
                m_TreadId,
                levelPadded,
                TraceDomain,
                TraceScope,
                message
                );
        }
        #endregion


        private void CreateTraceEnvironment(string traceDomain, string traceScope, StreamWriter traceStreamWriter = null)
        {
            ConsistencyCheck.EnsureArgument(traceDomain).IsNotNull();
            ConsistencyCheck.EnsureArgument(traceScope).IsNotNull();

            TraceDomain = traceDomain;
            TraceScope = traceScope;

            if (traceStreamWriter == null)
            {
                _folderProvider ??= new Solid.Infrastructure.Environment.Impl.FolderProvider();

                // we setup a new trace file which should relate to current application name and date/time of creation
                var traceFileName = _folderProvider.GetNewAppTraceFile();

                m_TraceStreamWriter = new StreamWriter(traceFileName);
                ConsistencyCheck.EnsureValue(m_TraceStreamWriter).IsNotNull();

                Console.WriteLine($"Created new TraceFile '{traceFileName}'"); //+ $" ({this.GetType().FullName})");
            }
            else
            {
                m_KeepStreamWriterAlive = true;
                m_TraceStreamWriter = traceStreamWriter;
            }

            m_CreationTime = DateTime.Now;
            m_ProcessId = Process.GetCurrentProcess().Id;
            //m_TreadId = Thread.CurrentThread.ManagedThreadId;

            #pragma warning disable 618
            m_TreadId = AppDomain.GetCurrentThreadId();
            #pragma warning restore 618

            // entering trace
            m_TraceStreamWriter.WriteLine("{0} {1}/{2} #*[ entering  {3} {4}",
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
                string.Format("{0} ms", System.Math.Round(timeSpan.TotalMilliseconds)):
                string.Format("{0} us", System.Math.Round(1000d * timeSpan.TotalMilliseconds));

            // leaving trace
            m_TraceStreamWriter?.WriteLine("{0} {1}/{2} #*] leaving   {3} {4} -> duration={5}",
                now.ToString("HH:mm:ss.ffffff"),
                m_ProcessId,
                m_TreadId,
                TraceDomain,
                TraceScope,
                spentTime
                );

            if (m_KeepStreamWriterAlive)
            {
                m_TraceStreamWriter?.Flush();
            }
            else if (m_TraceStreamWriter != null)
            {
                m_TraceStreamWriter.Close();
                m_TraceStreamWriter.Dispose();
                m_TraceStreamWriter = null;
            }
        }

        private bool m_IsDisposed;
        private bool m_KeepStreamWriterAlive;
        private StreamWriter m_TraceStreamWriter;
        private DateTime m_CreationTime;
        private int m_TreadId;
        private int m_ProcessId;
    }
}
