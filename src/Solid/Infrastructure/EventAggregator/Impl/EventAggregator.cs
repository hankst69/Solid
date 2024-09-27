//----------------------------------------------------------------------------------
// File: "EventAggregator.cs"
// Author: Steffen Hanke
// Date: 2016-2022
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Solid.Infrastructure.Diagnostics;

namespace Solid.Infrastructure.EventAggregator.Impl
{
    /// <summary>
    /// EventAggregator
    /// </summary>
    public class EventAggregator : IEventAggregator, IDumpable
    {
        private readonly IDictionary<Type, IList<ISubscriber>> _subscriptions = new Dictionary<Type, IList<ISubscriber>>();
        private ulong _publishNo;
        private readonly ITracer _tracer;

        /// <summary>
        /// Constructs a new instance of EventAggregator
        /// </summary>
        /// <param name="tracer">interface for trace outputs</param>
        public EventAggregator(ITracer tracer = null)
        {
            _tracer = tracer;
            using var ctorTrace = _tracer?.CreateScopeTracer();
        }

        /// <summary>
        /// Publishes <code>theEvent</code> in case no current <code>Publish</code> call is currently running.
        /// In the latter (reentrant) case, publishing the event will be deferred by means of a <code>ConcurrentQueue</code>
        /// </summary>
        /// <param name="theEvent">Event to be published</param>
        public void Publish(IEvent theEvent)
        {
            using var trace = _tracer?.CreateScopeTracer();
            if (theEvent == null)
            {
                throw new ArgumentNullException("theEvent");
            }

            // todo: add performance counter
            var messageType = theEvent.GetType();
            var publishNo = unchecked(_publishNo++);

            trace?.Info($"Publishing #{publishNo} event {theEvent}");

            IList<ISubscriber> subscriberList;
            if (_subscriptions.TryGetValue(messageType, out subscriberList))
            {
                // Notify all the subscribers
                foreach (var subscriber in subscriberList)
                {
                    subscriber.Notify(publishNo, theEvent);
                }
            }

            var interfaceSubscriberList = _subscriptions
                .Where(x =>
                {
                    if (messageType == x.Key)
                    {
                        return false;
                    }

                    var intf = messageType.GetInterface(x.Key.Name);
                    return intf != null && intf.FullName.Equals(x.Key.FullName);
                })
                .SelectMany(x => x.Value)
                .ToList();

            // Notify all the subscribers
            foreach (var subscriber in interfaceSubscriberList)
            {
                subscriber.Notify(publishNo, theEvent);
            }

            var subscriberCount = (subscriberList == null) ? 0 : subscriberList.Count;
            subscriberCount += interfaceSubscriberList.Count;

            trace?.Info($"Published {publishNo} {messageType.Name} to {subscriberCount} subscribers");
        }

        public IDisposable Subscribe<TEvent>(IHandleEvent<TEvent> eventHandler)
            where TEvent : IEvent
        {
            using var trace = _tracer?.CreateScopeTracer();
            if (eventHandler == null)
            {
                throw new ArgumentNullException("eventHandler");
            }

            // todo: add performance counter 
            var messageType = typeof(TEvent);
            var subscriber = new Subscriber<TEvent>(this, eventHandler, _tracer.CreateSubDomainTracer(string.Concat("Subscriber<", typeof(TEvent).Name, ">") /*typeof(Subscriber<TEvent>).FullName*/));

            IList<ISubscriber> subscriberList;
            if (_subscriptions.TryGetValue(messageType, out subscriberList))
            {
                if (subscriberList.FirstOrDefault(s => s.IsSameEventHandler(eventHandler)) == null)
                {
                    subscriberList.Add(subscriber);
                }
            }
            else
            {
                subscriberList = new List<ISubscriber> {subscriber};
                _subscriptions.Add(messageType, subscriberList);
            }

            trace?.Info($"Subscribed {subscriber} for {messageType.Name}");

            return subscriber;
        }

        public void UnSubscribeAll(object eventHandler)
        {
            using var trace = _tracer?.CreateScopeTracer();
            if (eventHandler == null)
            {
                throw new ArgumentNullException("eventHandler");
            }

            // search for all subscriptions with the given event handler
            foreach (var subscription in _subscriptions)
            {
                IList<ISubscriber> subscriberList = subscription.Value;
                ISubscriber subscriber = subscriberList.FirstOrDefault(s => s.IsSameEventHandler(eventHandler));
                if (subscriber != null)
                {
                    subscriberList.Remove(subscriber);
                    trace?.Info($"Unsubscribed {eventHandler} from {subscription.Key.Name}");
                }
            }
        }

        private void UnSubscribe<TEvent>(Subscriber<TEvent> subscriber)
            where TEvent : IEvent
        {
            using var trace = _tracer?.CreateScopeTracer();
            if (subscriber == null)
            {
                throw new ArgumentNullException("subscriber");
            }

            // todo: add performance counter 
            var messageType = typeof(TEvent);

            IList<ISubscriber> subscriberList;
            if (_subscriptions.TryGetValue(messageType, out subscriberList))
            {
                subscriberList.Remove(subscriber);
            }

            trace?.Info($"Unsubscribed {subscriber} from {messageType.Name}");
        }

        private interface ISubscriber
        {
            void Notify(ulong publishNo, IEvent theEvent);
            bool IsSameEventHandler(object eventHandler);
        }

        private sealed class Subscriber<TEvent> : ISubscriber, IDisposable
            where TEvent : IEvent
        {
            private readonly EventAggregator _eventAggregator;
            private readonly IHandleEvent<TEvent> _eventHandler;
            private readonly ITracer _tracer;
            private readonly Lazy<string> _eventName;
            private readonly Lazy<string> _subscriberName;

            public Subscriber(EventAggregator eventAggregator, IHandleEvent<TEvent> eventHandler, ITracer tracer)
            {
                _eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator");
                _eventHandler = eventHandler ?? throw new ArgumentNullException("eventHandler");
                _tracer = tracer;

                _eventName = new Lazy<string>(() => typeof(TEvent).Name);
                _subscriberName = new Lazy<string>(() => _eventHandler.GetType().Name);
            }

            public void Notify(ulong publishNo, IEvent theEvent)
            {
                using var trace = _tracer?.CreateScopeTracer();
                trace?.Info($"Publishing {publishNo} {_eventName.Value} to {_subscriberName.Value}");
                _eventHandler.Handle((TEvent) theEvent);
            }

            bool ISubscriber.IsSameEventHandler(object eventHandler)
            {
                return ReferenceEquals(eventHandler, _eventHandler);
            }

            public void Dispose()
            {
                _eventAggregator.UnSubscribe(this);
            }

            public override string ToString()
            {
                return _subscriberName.Value;
            }
        }

        public object Dump()
        {
            return _subscriptions.Select(s => new EventSubscriptions
            {
                EventType = s.Key.Name,
                Subscribers = Enumerable.Range(0, s.Value.Count).Select(i => s.Value[i].ToString()).ToList()
            }).ToList();
        }

        [DataContract]
        private class EventSubscriptions
        {
            [DataMember]
            public List<string> Subscribers { get; set; }

            [DataMember]
            public string EventType { get; set; }
        }

    }
}