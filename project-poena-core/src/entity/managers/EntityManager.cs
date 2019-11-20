using System;
using System.Collections.Generic;
using System.Linq;
using Project_Poena.Entity.Components;
using Project_Poena.Entity.Entities;

namespace Project_Poena.Entity.Managers
{

    public class EntityManager
    {
        public List<ECEntity> entities { get; private set; }

        public EntityManager()
        {
            this.entities = new List<ECEntity>();
        }

        public void AddEntity(ECEntity e)
        {
            this.entities.Add(e);
        }

        public void RemoveEntity(ECEntity e)
        {
            this.entities.Remove(e);
        }

        public List<ECEntity> GetEntities(params Type[] componentTypes)
        {
            return this.GetEntities(false, componentTypes);
        }

        public List<ECEntity> GetEntities(bool isStrict, params Type[] componentTypes)
        {
            Func<ECEntity, bool> func;

            if (isStrict) func = ent => ent.HasAllComponents(componentTypes);
            else func = ent => ent.HasComponent(componentTypes);

            return this.entities.Where(func).ToList();
        }

        public ECEntity GetEntity(params Type[] componentTypes)
        {
            return this.entities.FirstOrDefault(ent => ent.HasAllComponents(componentTypes));
        }

        public ECEntity GetSelectedEntity()
        {
            return this.entities.FirstOrDefault(ent => ent.HasComponent(typeof(SelectedComponent)));
        }

        public void Update(double dt)
        {
            //Remove any flagged for removals
            this.entities.RemoveAll(ent => ent.IsFlaggedForRemoval());
        }

    }

}
