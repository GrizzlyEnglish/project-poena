using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Project_Poena.Entity.Systems;
using Project_Poena.Common.Rectangle;
using Microsoft.Xna.Framework.Content;
using Project_Poena.Scene;

namespace Project_Poena.Entity.Managers
{
    public class SystemManager
    {
        public Queue<ECSystem> systems { get; private set; }

        public EntityManager entity_manager { get; private set; }

        public AbstractScene scene { get; private set; }

        public SystemManager(AbstractScene scene)
        {
            this.scene = scene;
            this.entity_manager = scene.entity_manager;

            this.systems = new Queue<ECSystem>();
            
            this.systems.Enqueue(new PositionSystem(this));
            this.systems.Enqueue(new SelectionSystem(this));
            this.systems.Enqueue(new TileHighlightSystem(this));
            this.systems.Enqueue(new SkillSystem(this));

            //Always render entities last
            this.systems.Enqueue(new EntityRenderSystem(this));
            this.systems.Enqueue(new TurnSystem(this));
        }

        public void Initialize()
        {
            foreach (ECSystem system in systems)
            {
                system.Initiliaze();
            }
        }

        public void LoadContent(ContentManager contentManager)
        {
            //Load content for all entities
            new ContentSystem(this).LoadContent(contentManager);

            foreach (ECSystem system in systems)
            {
                system.LoadContent(contentManager);
            }
        }

        public void Message(string message, object data, bool notifyAll = false)
        {
            foreach (ECSystem system in systems)
            {
                bool accepted = system.RecieveMessage(message, data);
                if (!notifyAll && accepted) break;
            }
        }

        public void Update(double dt)
        {
            foreach (ECSystem system in systems)
            {
                system.Update(dt);
            }
        }

        public void Render(SpriteBatch batch, RectangleF camera_bounds)
        {
            foreach (ECSystem system in systems)
            {
                system.Render(batch, camera_bounds);
            }
        }

    }
}
