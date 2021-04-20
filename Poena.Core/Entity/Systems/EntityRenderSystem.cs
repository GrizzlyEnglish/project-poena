using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Poena.Core.Common;
using Poena.Core.Entity.Components;
using Poena.Core.Entity.Managers;

namespace Poena.Core.Entity.Systems
{
    public class EntityRenderSystem : ECSystem
    {
        public EntityRenderSystem(SystemManager systemManager) : base(systemManager)
        {

        }

        public override void Initiliaze()
        {
            
        }
        
        public override void Update(double dt)
        {
            List<ECEntity> entities =
                    this.Manager.EntityManager.GetEntities(new Type[] { typeof(SpriteComponent) });
            foreach (ECEntity ent in entities)
            {
                //TODO: rce - Add state logic
                SpriteComponent anim = ent.GetComponent<SpriteComponent>();

                //Add the dt to the animation
                anim.animation.Update(dt);
            }
        }

        public override void Render(SpriteBatch batch, RectangleF camera_bounds)
        {
            List<ECEntity> entities = 
                this.Manager.EntityManager.GetEntities(new Type[] { typeof(SpriteComponent), typeof(PositionComponent) });
            foreach(ECEntity entity in entities)
            {
                //TODO: rce - Add state to get the current animation of the state
                SpriteComponent anim = entity.GetComponent<SpriteComponent>();
                PositionComponent pos_comp = entity.GetComponent<PositionComponent>();

                //Get the tile anchor position
                Vector2 pos = pos_comp.tile_position;
                
                anim.animation.SetPosition(pos);
                anim.animation.Render(batch);
            }
        }
    }
}
