using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Project_Poena.Common.Rectangle;
using Project_Poena.Entity.Managers;
using Project_Poena.Input;
using System.Collections.Generic;

namespace Project_Poena.Entity.Systems
{
    public abstract class ECSystem
    {
        protected SystemManager manager;

        protected string identifier;

        public ECSystem(SystemManager systemManager)
        {
            this.manager = systemManager;
        }

        public void SendMessage(string message, object data)
        {
            this.manager.Message(message, data);
        }

        public virtual bool RecieveMessage(string message, object data)
        {
            return false;
        }

        public abstract void Initiliaze();

        //Overrideable
        public virtual List<MappedInputAction> HandleLayerInput(List<MappedInputAction> actions) { return actions; }
        public virtual void LoadContent(ContentManager contentManager) { }
        public virtual void Update(double dt) { }
        public virtual void Render(SpriteBatch batch, RectangleF camera_bounds) { }

    }
}
