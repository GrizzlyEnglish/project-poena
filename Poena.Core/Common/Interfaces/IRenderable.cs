using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poena.Core.Common.Interfaces
{
    public interface IRenderable
    {
        void LoadContent(ContentManager contentManager);
        void Render(SpriteBatch spriteBatch, RectangleF camera_bounds = null);
        StateEnum Update(double delta);
    }
}
