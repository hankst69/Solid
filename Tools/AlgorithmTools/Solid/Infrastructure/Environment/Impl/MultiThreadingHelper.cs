//----------------------------------------------------------------------------------
// <copyright file="MultiThreadingHelper.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2018-2023. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using Solid.Infrastructure.Diagnostics;

using System;
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

        public MultiThreadingHelper(ITracer tracer = null)
        {
            _tracer = tracer;
            using var trace = _tracer?.CreateScopeTracer();
        }

        public void Dispose()
        {
            using var trace = _tracer?.CreateScopeTracer();
        }

        public void ExecuteDelayed(Action action, int milliseconds)
        {
            using var trace = _tracer?.CreateScopeTracer();

            if (milliseconds < 0)
            {
                // negative delay would mean to wait indefinitely
                milliseconds = 0;
            }

            ////SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            ////var currentThreadSyncContext = SynchronizationContext.Current;
            ////var currentThreadTaskSheduler = TaskScheduler.FromCurrentSynchronizationContext();
            //var currentThreadTaskSheduler = TaskScheduler.Current;

            Task.Delay(milliseconds).ContinueWith(t =>
            {
                action();
            //}, currentThreadTaskSheduler);
            }, TaskContinuationOptions.ExecuteSynchronously);
        }

        //public void ExecuteDelayedInCurrentThread(Action action, int milliseconds) => ExecuteDelayed(action, milliseconds);
    }
}
