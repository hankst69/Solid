//----------------------------------------------------------------------------------
// File: "IHandleEvent.cs"
// Author: Steffen Hanke
// Date: 2017-2018
//----------------------------------------------------------------------------------

namespace Solid.Infrastructure.EventAggregator
{
    /// <summary>
    /// IHandleEvent
    /// </summary>
    public interface IHandleEvent<in TEvent> where TEvent : IEvent
    {
        void Handle(TEvent theEvent);
    }
}
