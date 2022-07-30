//----------------------------------------------------------------------------------
// <copyright file="IEventAggregator.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2017-2018. All Rights Reserved. Confidential.
// </copyright>
//----------------------------------------------------------------------------------

using System;

namespace Solid.Infrastructure.EventAggregator
{
    /// <summary>
    /// IEventAggregator
    /// </summary>
    public interface IEventAggregator
    {
        IDisposable Subscribe<TEvent>(IHandleEvent<TEvent> eventHandler) where TEvent : IEvent;
        void UnSubscribeAll(object eventHandler);

        void Publish(IEvent theEvent);
    }
}
