using System;
using System.Collections.Generic;

namespace Poena.Core.Events
{
    public static class EventPubSubHandler
    {
        private static List<EventSubscription> subscriptions = new List<EventSubscription>();

        private class EventSubscription
        {
            public string event_name { get; private set; }
            private LinkedList<Action<object>> subscribers;

            public EventSubscription(string name, Action<object> func)
            {
                this.event_name = name.ToLower();
                this.subscribers = new LinkedList<Action<object>>();
                this.AddFunction(func);
            }

            public void AddFunction(Action<object> func)
            {
                this.subscribers.AddLast(func);
            }

            public void RemoveFunction(Action<object> func)
            {
                this.subscribers.Remove(func);
            }

            public void Notify(object data)
            {
                foreach (Action<object> d in this.subscribers)
                {
                    d(data);
                }
            }
        }
        
        public static void Subscribe(string eventName, Action<object> func)
        {
            EventSubscription es = subscriptions.Find(sub => sub.event_name.Equals(eventName.ToLower()));
            if (es == null)
            {
                subscriptions.Add(new EventSubscription(eventName, func));
            }
            else
            {
                es.AddFunction(func);
            }
        }

        public static void Unsubscribe(string eventName, Action<object> func)
        {
            subscriptions.Find(sub => sub.event_name.Equals(eventName.ToLower()))?.RemoveFunction(func);
        }

        public static void Notify(string eventName, object data)
        {
            subscriptions.Find(sub => sub.event_name.Equals(eventName.ToLower()))?.Notify(data);
        }
    }
}
