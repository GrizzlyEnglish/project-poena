using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poena.Core.Common.Interfaces
{
    public interface IRenderable
    {
        void LoadContent(ContentManager contentManager);
        void Render(SpriteBatch spriteBatch, RectangleF cameraBounds);
        StateEnum Update(double delta);
    }
}
