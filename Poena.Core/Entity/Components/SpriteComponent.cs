using Microsoft.Xna.Framework;
using Poena.Core.Sprites;

namespace Poena.Core.Entity.Components
{
    public class SpriteComponent : Component
    {
        public Sprite Animation { get; set; }
        public bool IsVisible { get { return Animation.IsVisible; } set { Animation.IsVisible = value; } }

        public void NewSprite(params string[] paths)
        {
            this.NewSprite(null, paths);
        }

        public void NewSprite(Vector2? base_offset, params string[] paths)
        {
            this.Animation = new Sprite(paths);
            //Update the anchor to bottom center of base
            if (base_offset.HasValue) this.Animation.AnchorOffset = base_offset.Value;
        }
        
        public override void Initialize()
        {
            
        }

    }
}
