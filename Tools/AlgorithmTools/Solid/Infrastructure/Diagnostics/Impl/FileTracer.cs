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


        #region ITracerCreator
        protected override ITracer CreateBaseDomainTracer(string traceDomainName)
        {
            ConsistencyCheck.EnsureArgument(traceDomainName).IsNotNullOrEmpty();
            return traceDomainName.Equals(TraceDomain) ? this : new FileTracer(traceDomainName, string.Empty, _traceStreamWriter);
        }

        public override ITracer CreateSubDomainTracer(string subDomain)
        {
            ConsistencyCheck.EnsureArgument(subDomain).IsNotNull();
            var traceDomain = string.IsNullOrEmpty(TraceDomain) ? subDomain : string.Concat(TraceDomain, "+", subDomain);
            return new FileTracer(traceDomain, string.Empty, _traceStreamWriter);
        }

        public override ITracer CreateScopeTracer(string scopeName)
        {
            return new FileTracer(TraceDomain, scopeName, _traceStreamWriter);
        }
        #endregion


        #region ILogger
        protected override void WriteTrace(string level, string message)
        {
            var levelPadded = level.PadRight(9);

            _traceStreamWriter?.WriteLine("{0} {1}/{2} #** {3} {4} {5} -> {6}<-",
                DateTime.Now.ToString("HH:mm:ss.ffffff"),
                _processId,
                _threadId,
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

                _traceStreamWriter = new StreamWriter(traceFileName);
                ConsistencyCheck.EnsureValue(_traceStreamWriter).IsNotNull();

                Console.WriteLine($"Created new TraceFile '{traceFileName}'"); //+ $" ({this.GetType().FullName})");
            }
            else
            {
                _keepStreamWriterAlive = true;
                _traceStreamWriter = traceStreamWriter;
            }

            _creationTime = DateTime.Now;
            _processId = Process.GetCurrentProcess().Id;
            //_threadId = Thread.CurrentThread.ManagedThreadId;

            #pragma warning disable 618
            _threadId = AppDomain.GetCurrentThreadId();
            #pragma warning restore 618

            // write entering trace
            if (IsTraceLevel(TraceLevel.InOut))
            {
                _traceStreamWriter.WriteLine("{0} {1}/{2} #*[ entering  {3} {4}",
                    _creationTime.ToString("HH:mm:ss.ffffff"),
                    _processId,
                    _threadId,
                    TraceDomain,
                    TraceScope
                    );
            }
        }

        protected override void DisposeTraceEnvironment()
        {
            var now = DateTime.Now;
            var timeSpan = now - _creationTime;
            var spentTime = timeSpan.TotalMilliseconds > 9 ? 
                string.Format("{0} ms", System.Math.Round(timeSpan.TotalMilliseconds)):
                string.Format("{0} us", System.Math.Round(1000d * timeSpan.TotalMilliseconds));

            // write leaving trace
            if (IsTraceLevel(TraceLevel.InOut))
            {
                _traceStreamWriter?.WriteLine("{0} {1}/{2} #*] leaving   {3} {4} -> duration={5}",
                    now.ToString("HH:mm:ss.ffffff"),
                    _processId,
                    _threadId,
                    TraceDomain,
                    TraceScope,
                    spentTime
                    );
            }

            if (_keepStreamWriterAlive)
            {
                _traceStreamWriter?.Flush();
            }
            else if (_traceStreamWriter != null)
            {
                _traceStreamWriter.Close();
                _traceStreamWriter.Dispose();
                _traceStreamWriter = null;
            }
        }

        private bool _keepStreamWriterAlive;
        private StreamWriter _traceStreamWriter;
        private DateTime _creationTime;
        private int _threadId;
        private int _processId;
    }
}
