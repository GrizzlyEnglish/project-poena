using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Poena.Core.Common;
using Poena.Core.Entity.Systems;
using Poena.Core.Scene;
using Poena.Core.Scene.Battle.Systems;

namespace Poena.Core.Entity.Managers
{
    public class SystemManager
    {
        public Queue<ECSystem> Systems { get; private set; }

        public EntityManager EntityManager { get; private set; }

        public SceneLayer SceneLayer { get; private set; }

        public SystemManager(SceneLayer sceneLayer)
        {
            this.SceneLayer = sceneLayer;
            this.EntityManager = sceneLayer.EntityManager;

            this.Systems = new Queue<ECSystem>();
            
            this.Systems.Enqueue(new PositionSystem(this));
            this.Systems.Enqueue(new SelectionSystem(this));
            this.Systems.Enqueue(new TileHighlightSystem(this));
            this.Systems.Enqueue(new SkillSystem(this));

            //Always render entities last
            this.Systems.Enqueue(new EntityRenderSystem(this));
            this.Systems.Enqueue(new TurnSystem(this));
        }

        public void Initialize()
        {
            foreach (ECSystem Systems in Systems)
            {
                Systems.Initiliaze();
            }
        }

        public void LoadContent(ContentManager contentManager)
        {
            //Load content for all entities
            new ContentSystem(this).LoadContent(contentManager);

            foreach (ECSystem Systems in Systems)
            {
                Systems.LoadContent(contentManager);
            }
        }

        public void Message(string message, object data, bool notifyAll = false)
        {
            foreach (ECSystem Systems in Systems)
            {
                bool accepted = Systems.RecieveMessage(message, data);
                if (!notifyAll && accepted) break;
            }
        }

        public void Update(double dt)
        {
            foreach (ECSystem Systems in Systems)
            {
                Systems.Update(dt);
            }
        }

        public void Render(SpriteBatch batch, RectangleF camera_bounds)
        {
            foreach (ECSystem Systems in Systems)
            {
                Systems.Render(batch, camera_bounds);
            }
        }

    }
}
