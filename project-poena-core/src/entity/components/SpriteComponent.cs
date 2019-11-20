using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project_Poena.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project_Poena.Entity.Components
{
    public class SpriteComponent : Component
    {
        public Sprite animation { get; set; }
        public bool is_visible { get { return animation.is_visible; } set { animation.is_visible = value; } }

        public void NewSprite(params string[] paths)
        {
            this.NewSprite(null, paths);
        }

        public void NewSprite(Vector2? base_offset, params string[] paths)
        {
            this.animation = new Sprite(paths);
            //Update the anchor to bottom center of base
            if (base_offset.HasValue) this.animation.anchor_offset = base_offset.Value;
        }
        
        public override void Initialize()
        {
            
        }

    }
}
