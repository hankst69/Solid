//----------------------------------------------------------------------------------
// File: "IEventAggregator.cs"
// Author: Steffen Hanke
// Date: 2017-2018
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
