//----------------------------------------------------------------------------------
// <copyright file="MultiThreadingHelper.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2018-2023. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using Solid.Infrastructure.Diagnostics;

using System;
//using System.Threading;
using System.Threading.Tasks;


namespace Solid.Infrastructure.Environment.Impl
{
    /// <inheritdoc cref="IMultiThreadingHelper" />
    /// <inheritdoc cref="IDisposable" />
    /// <summary>
    /// API:NO
    /// MultiThreadingHelper
    /// </summary>
    public class MultiThreadingHelper : IMultiThreadingHelper, IDisposable
    {
        private readonly ITracer _tracer;

        public MultiThreadingHelper()
        {
        }

        public MultiThreadingHelper(ITracer tracer)
        {
            ConsistencyCheck.EnsureArgument(tracer).IsNotNull();
            using var trace = _tracer.CreateScopeTracer();
        }

        public void Dispose()
        {
            using var trace = _tracer?.CreateScopeTracer();
        }

        public void ExecuteDelayedInCurrentThread(Action action, int milliseconds)
        {
            using var trace = _tracer?.CreateScopeTracer();

            //var currentThreadSyncContext = SynchronizationContext.Current;
            //var currentThreadTaskSheduler = TaskScheduler.FromCurrentSynchronizationContext();
            var currentThreadTaskSheduler = TaskScheduler.Current;
            if (milliseconds < 0)
            {
                // negative delay would mean to wait indefinitely
                milliseconds = 0;
            }
            Task.Delay(milliseconds).ContinueWith(t =>
            {
                //var currentTaskScheduler = TaskScheduler.Current;
                action();
            }, currentThreadTaskSheduler);
        }

    }
}
