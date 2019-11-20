using Project_Poena.Extensions;
using System.Collections.Generic;

namespace Project_Poena.Events
{
    public static class EventQueueHandler
    {
        private static EventHandler event_handler;

        public class EventHandler
        {
            private List<Event> queued_events;

            public EventHandler()
            {
                queued_events = new List<Event>();
            }

            //Do a full empty of the queued events
            public void Clear()
            {
                queued_events.Clear();
            }

            // Clears unused events in namespace
            public void ClearNamespace(string event_namespace)
            {
                //TODO: rce - Do we need an event that stays until digested?
                queued_events.ForEach(evt => {
                    if (evt.event_namespace == event_namespace && !evt.IsFlagged())
                    {
                        //This was not used flag it for removal
                        evt.HandleEvent();
                    }
                });
            }

            public void QueueEvent(Event evt)
            {
                //Before adding remove all the handled events
                queued_events.RemoveHandled();
                //Now we can add it
                queued_events.Add(evt);
            }

            //Finds all the scene layer events and returns them by removing them
            public List<Event> GetEvents(string event_namespace, string event_name = null)
            {
                List<Event> selected_events = new List<Event>();
                for (int i = 0; i < queued_events.Count; i++)
                {
                    Event e = queued_events[i];

                    if (e.IsFlagged())
                    {
                        queued_events.Remove(e);
                    }
                    else if (e.event_namespace.ToLower() == event_namespace &&
                        (event_name == null || event_name.ToLower() == e.name))
                    {
                        selected_events.Add(e);
                    }
                }
                return selected_events;
            }

            //Find the first
            public Event GetEvent(string event_namespace, string event_name = null)
            {
                queued_events.RemoveHandled();

                foreach (Event e in queued_events)
                {
                    if (e.event_namespace.ToLower() == event_namespace &&
                        (event_name == null || event_name.ToLower() == e.name))
                    {
                        return e;
                    }
                }
                return null;
            }
        }

        public static EventHandler GetInstance()
        {
            if (event_handler == null) event_handler = new EventHandler();

            return event_handler;
        }
    }
}
