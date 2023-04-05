//----------------------------------------------------------------------------------
// <copyright file="IHandleEvent.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2017-2018. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
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
