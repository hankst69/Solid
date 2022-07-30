//----------------------------------------------------------------------------------
// <copyright file="BaseTracer.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2019-2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Solid.Infrastructure.Diagnostics.Impl
{
    public abstract class BaseTracer : ILogger
    {
        protected virtual ITracer CreateBaseDomainTracer(string traceDomainName) { return null; }

        protected virtual void WriteTrace(string level, string message) { }


        #region ITracerInfo
        public string TraceDomain { get; protected set; }
        public string TraceScope { get; protected set; }
        #endregion


        #region ILogger
        public void Error(string message, string callerName, int callerLine, string callerFilePath)
        {
            WriteTrace("Error", message);
        }

        public void Error(Exception ex, string callerName, int callerLine, string callerFilePath)
        {
            WriteTrace("Error", string.Format("{0}", ex));
        }

        public void Info(string message, string callerName, int callerLine, string callerFilePath)
        {
            WriteTrace("Info", message);
        }

        public void Warning(string message, string callerName, int callerLine, string callerFilePath)
        {
            WriteTrace("Warning", message);
        }

        public void Debug(string message, string callerName, int callerLine, string callerFilePath)
        {
            WriteTrace("Debug", message);
        }
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
        #endregion
    }
}
