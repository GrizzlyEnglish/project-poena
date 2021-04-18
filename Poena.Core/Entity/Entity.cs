using System;
using System.Collections.Generic;
using System.Linq;
using Poena.Core.Entity.Components;

namespace Poena.Core.Entity
{
    public class ECEntity
    {
        public List<Component> components { get; private set; }

        private bool is_flagged_for_removal;

        public ECEntity()
        {
            components = new List<Component>();
        }

        public void AddComponent(Component component)
        {
            //TODO: rce - Add logic to check if component already exists

            //Initialize the component
            component.Initialize();

            //Add it to the entity
            this.components.Add(component);
        }

        public void AddComponent(params Component[] component)
        {
            foreach(Component comp in component)
            {
                this.AddComponent(comp);
            }
        }

        public void RemoveComponent(params Component[] components)
        {
            this.components.RemoveAll(comp => components.Contains(comp));
        }

        public void RemoveComponent(params Type[] components)
        {
            this.components.RemoveAll(comp => components.Contains(comp.GetType()));
        }

        public void GetComponent(Component component)
        {
            this.components.Remove(component);
        }

        public T GetComponent<T>() where T : Component
        {
            foreach (Component comp in components)
            {
                if (comp.GetType() == typeof(T))
                {
                    return (T)comp;
                }
            }

            return null;
        }
        
        public bool HasComponent(Type componentType)
        {
            return this.components.Any(comp => componentType == comp.GetType());
        }

        public bool HasComponent(Type[] componentTypes)
        {
            return this.components.Any(comp => componentTypes.Contains(comp.GetType()));
        }

        public bool HasAllComponents(Type[] componentTypes)
        {
            List<Type> components = this.components.Select(comp => comp.GetType()).ToList();
            return componentTypes.All(ct => components.Contains(ct));
        }

        public void FlagForRemoval()
        {
            this.is_flagged_for_removal = true;
        }

        public bool IsFlaggedForRemoval()
        {
            return this.is_flagged_for_removal;
        }

    }
}
