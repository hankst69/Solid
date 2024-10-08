﻿//----------------------------------------------------------------------------------
// File: "ConsoleTracer.cs"
// Author: Steffen Hanke
// Date: 2020-2023
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
            ConsistencyCheck.EnsureArgument(message).IsNotNull();

            var isError = message.Contains(" #** Error ");
            var isWarning = !isError && message.Contains(" #** Warning ");
            var isDebug = !isError && !isWarning && message.Contains(" #** Debug ");
            //var isInfo = !isError && !isWarning && !isDebug && message.Contains(" #** Info ");

            if (message.EndsWith(" <-"))
            {
                message = message.Substring(0, message.Length - 3);
            }
            if (message.Length > 160)
            {
                message = message.Replace("-> ", "->\n ");
            }

            var fgColor = Console.ForegroundColor;
            var newFgColor = fgColor;
            newFgColor = isError ? ConsoleColor.Red : newFgColor;
            newFgColor = isWarning ? ConsoleColor.DarkYellow : newFgColor;
            newFgColor = isDebug ? ConsoleColor.DarkGreen : newFgColor;
            Console.ForegroundColor = newFgColor;

            if (isError) 
            {
                Console.Error.WriteLine(message);
                Console.Error.Flush();
            }
            else
            {
                Console.Out.WriteLine(message);
                Console.Out.Flush();
            }

            Console.ForegroundColor = fgColor;
        }


        #region ITracerCreator
        protected override ITracer CreateBaseDomainTracer(string traceDomainName)
        {
            ConsistencyCheck.EnsureArgument(traceDomainName).IsNotNullOrEmpty();
            return TraceDomain.Equals(traceDomainName) ? this : new ConsoleTracer(traceDomainName, string.Empty)
            {
                TraceLevel = TraceLevel,
                TraceScope = "_"
            }.WriteEnterTrace();
        }

        public override ITracer CreateSubDomainTracer(string subDomain)
        {
            ConsistencyCheck.EnsureArgument(subDomain).IsNotNull();
            var traceDomain = string.IsNullOrEmpty(TraceDomain) ? subDomain : string.Concat(TraceDomain, "+", subDomain);
            return new ConsoleTracer(traceDomain, string.Empty)
            {
                TraceLevel = TraceLevel,
                TraceScope = "_"
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
