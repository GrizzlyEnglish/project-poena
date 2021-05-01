using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Poena.Core.Entity.Components;

namespace Poena.Core.Screen.Battle.Components
{
    public class SpriteComponent : IComponent
    {
        public float Scale { get; set; } = 1f;
        public Texture2D Texture { get; set; }
        public bool IsVisible { get; set; }

        public float Width
        {
            get
            {
                return this.Texture.Width * this.Scale;
            }
        }

        public float Height
        {
            get
            {
                return this.Texture.Height * this.Scale;
            }
        }
        
        public Vector2 Anchor { 
            get { 
                return new Vector2((this.Texture.Width * this.Scale) * AnchorOffset.X, (this.Texture.Height * this.Scale) * AnchorOffset.Y); 
            } 
        }
        private Vector2? _AnchorOffset { get; set; }
        public Vector2 AnchorOffset
        {
            get
            {
                return _AnchorOffset ?? new Vector2(0, 0);
            }
            set
            {
                this._AnchorOffset = value;
            }
        }

        public void Initialize()
        {
        }

    }
}
