using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Poena.Core.Common;
using Poena.Core.Entity.Managers;

namespace Poena.Core.Entity.Systems
{
    public abstract class ECSystem
    {
        protected SystemManager Manager;

        protected string identifier;

        public ECSystem(SystemManager systemManager)
        {
            this.Manager = systemManager;
        }

        public void SendMessage(string message, object data)
        {
            this.Manager.Message(message, data);
        }

        public virtual bool RecieveMessage(string message, object data)
        {
            return false;
        }

        public abstract void Initiliaze();
        public virtual void LoadContent(ContentManager contentManager) { }
        public virtual void Update(double dt) { }
        public virtual void Render(SpriteBatch batch, RectangleF cameraBounds) { }

    }
}
