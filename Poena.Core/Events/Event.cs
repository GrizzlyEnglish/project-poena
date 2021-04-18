using Poena.Core.Common;

namespace Poena.Core.Events
{
    public class Event : IRemovable
    {
        public string event_namespace { get; private set; }
        public string name { get; private set; }
        public object data { get; private set; }

        private bool handled_event { get; set; }

        public Event(string event_namespace, string name, object data)
        {
            this.event_namespace = event_namespace;
            this.name = name;
            this.data = data;
        }

        public void HandleEvent()
        {
            this.handled_event = true;
        }

        public bool IsFlagged()
        {
            return this.handled_event;
        }
    }
}
