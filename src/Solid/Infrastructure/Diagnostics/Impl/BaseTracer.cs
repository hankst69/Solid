//----------------------------------------------------------------------------------
// <copyright file="BaseTracer.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2019-2023. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Linq;

namespace Solid.Infrastructure.Diagnostics.Impl
{
    public abstract class BaseTracer : ITracer
    {
        private bool _isDisposed;
        private DateTime _creationTime;
        private int _threadId;
        private int _processId;
        private readonly int _levelPadding = //9;
            ((TraceLevel[])Enum.GetValues(typeof(TraceLevel)))
            .Select(x => x == TraceLevel.InOut? 0 : x.ToString().Length)
            .Max() + 2;
        
        protected virtual void WriteTraceEntry(string message) { }

        protected ITracer WriteEnterTrace()
        {
            _creationTime = DateTime.Now;

            // write entering trace
            if (IsTraceLevel(TraceLevel.InOut))
            {
                _processId = Process.GetCurrentProcess().Id;
                //_threadId = Thread.CurrentThread.ManagedThreadId;
                #pragma warning disable 618
                _threadId = AppDomain.GetCurrentThreadId();
                #pragma warning restore 618

                var traceEntry = $"{_creationTime.ToString("HH:mm:ss.ffffff")} {_processId}/{_threadId} #*[ entering  {TraceDomain} {TraceScope}";
                WriteTraceEntry(traceEntry);
            }
            return this;
        }

        private void WriteLeaveTrace()
        {
            // write leaving trace
            if (IsTraceLevel(TraceLevel.InOut))
            {
                var now = DateTime.Now;
                var timeSpan = now - _creationTime;
                var spentTime = timeSpan.TotalMilliseconds > 9 ?
                    string.Format("{0} ms", System.Math.Round(timeSpan.TotalMilliseconds)) :
                    string.Format("{0} us", System.Math.Round(1000d * timeSpan.TotalMilliseconds));

                _processId = _processId == 0 ? Process.GetCurrentProcess().Id : _processId;
                #pragma warning disable 618
                _threadId = _threadId == 0 ? AppDomain.GetCurrentThreadId() : _threadId;
                #pragma warning restore 618

                var traceEntry = $"{now.ToString("HH:mm:ss.ffffff")} {_processId}/{_threadId} #*] leaving   {TraceDomain} {TraceScope} -> duration={spentTime}";
                WriteTraceEntry(traceEntry);
            }
        }

        protected void CreateTraceEnvironment(string traceDomain, string traceScope)
        {
            ConsistencyCheck.EnsureArgument(traceDomain).IsNotNull();
            ConsistencyCheck.EnsureArgument(traceScope).IsNotNull();

            TraceDomain = traceDomain;
            TraceScope = traceScope;

            //WriteEnterTrace();
        }

        protected virtual void DisposeTraceEnvironment() { }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;

            WriteLeaveTrace();
            DisposeTraceEnvironment();
        }

        private bool IsTraceLevel(TraceLevel traceLevel) => (TraceLevel & traceLevel) > 0;

        private void WriteTraceInternal(TraceLevel level, string message)
        {
            if (IsTraceLevel(level))
            {
                _processId = _processId == 0 ? Process.GetCurrentProcess().Id : _processId;
                #pragma warning disable 618
                _threadId = _threadId == 0 ? AppDomain.GetCurrentThreadId() : _threadId;
                #pragma warning restore 618

                var levelPadded = level.ToString().PadRight(_levelPadding);

                var traceEntry = $"{DateTime.Now.ToString("HH:mm:ss.ffffff")} {_processId}/{_threadId} #** {levelPadded} {TraceDomain} {TraceScope} -> {message} <-";

                WriteTraceEntry(traceEntry);
            }
        }


        #region ITracerInfo
        public string TraceDomain { get; protected set; }
        public string TraceScope { get; protected set; }
        public TraceLevel TraceLevel { get; set; }
        #endregion


        #region ILogger
        public void Error(string message, string callerName, int callerLine, string callerFilePath) 
            => WriteTraceInternal(TraceLevel.Error, message);

        public void Error(Exception ex, string callerName, int callerLine, string callerFilePath) 
            => WriteTraceInternal(TraceLevel.Error, string.Format("{0}", ex));

        public void Info(string message, string callerName, int callerLine, string callerFilePath) 
            => WriteTraceInternal(TraceLevel.Info, message);

        public void Warning(string message, string callerName, int callerLine, string callerFilePath) 
            => WriteTraceInternal(TraceLevel.Warning, message);

        public void Debug(string message, string callerName, int callerLine, string callerFilePath) 
            => WriteTraceInternal(TraceLevel.Debug, message);
        #endregion


        #region ITracerCreator
        public ITracer CreateBaseDomainTracer() => CreateBaseDomainTracer(ReadTraceDomainFromCallStack());

        public ITracer CreateBaseDomainTracer(Type traceDomainType)
        {
            ConsistencyCheck.EnsureArgument(traceDomainType).IsNotNull();
            return CreateBaseDomainTracer(GetTraceDomainNameFromTraceDomainType(traceDomainType));
        }

        protected static string ReadTraceDomainFromCallStack()
        {
            return GetTraceDomainNameFromTraceDomainType(ReadTraceDomainTypeFromCallStack());
        }

        protected static string GetTraceDomainNameFromTraceDomainType(Type traceDomainType)
        {
            if (traceDomainType == typeof(void))
            {
                return string.Empty;
            }
            return traceDomainType.FullName;
        }

        protected static Type ReadTraceDomainTypeFromCallStack()
        {
            // try to detect DomainName from TypeName of our creator by walking up the stack
            var stackTrace = new StackTrace();

            // Todo: implement new strategy:
            // 1) search callstack up for last occurence of 'CreateBaseDomainTracer()'
            //    if such a member was found then then return the next StackFrame 
            // else 2) search callstack up for a Solid.DiContainer member (look for the type that resolved us as a parameter)
            //    if such a member was found then go up further until last member of Solid.DiContainer was used, then return the next StackFrame
            // else 3) take the Stackframe based on numStackFramesUp
            var frameIdx = stackTrace.FrameCount - 1;
            // 1) search callstack up for last occurence of 'CreateBaseDomainTracer()'
            while (frameIdx >= 0
                && stackTrace.GetFrame(frameIdx)?.GetMethod()?.Name != "CreateBaseDomainTracer")
            {
                frameIdx--;
            }
            if (frameIdx >= 0 && (frameIdx+1) < stackTrace.FrameCount)
            {
                frameIdx++;
                return stackTrace.GetFrame(frameIdx)?.GetMethod()?.DeclaringType ?? typeof(void);
            }

            // else 2) search callstack up for a Solid.DiContainer member (look for the type that resolved us as a parameter)
            frameIdx = 0;
            while (frameIdx < stackTrace.FrameCount
                && stackTrace.GetFrame(frameIdx)?.GetMethod()?.DeclaringType != typeof(Solid.Infrastructure.DiContainer.Impl.DiContainer))
            {
                frameIdx++;
            }
            if (frameIdx < stackTrace.FrameCount)
            {
                while (frameIdx < stackTrace.FrameCount
                    && stackTrace.GetFrame(frameIdx)?.GetMethod()?.DeclaringType == typeof(Solid.Infrastructure.DiContainer.Impl.DiContainer))
                {
                    frameIdx++;
                }
                if (frameIdx < stackTrace.FrameCount)
                {
                    return stackTrace.GetFrame(frameIdx)?.GetMethod()?.DeclaringType ?? typeof(void);
                }
            }

            // else 3) take the Stackframe based on numStackFramesUp
            //frameIdx 0 is ourself (this function), frameIdx 1 is 1 up (ReadTraceDomainFromCallStack), frameIdx 2 is 2 up (our ctor), frameIdx 3 is 3 up (our creator)
            var numStackFramesUp = 3;
            var creatorType = numStackFramesUp < stackTrace.FrameCount ? stackTrace.GetFrame(numStackFramesUp)?.GetMethod()?.DeclaringType : null;
            return creatorType ?? typeof(void);
        }

        protected virtual ITracer CreateBaseDomainTracer(string traceDomainName) { return null; }
        public abstract ITracer CreateSubDomainTracer(string subDomain);
        public abstract ITracer CreateScopeTracer([CallerMemberName] string scopeName = "");
        #endregion
    }
}
