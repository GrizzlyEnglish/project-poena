using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Poena.Core.Common;
using Poena.Core.Entity.Components;
using Poena.Core.Entity.Managers;
using Poena.Core.Scene.Battle.Components;

namespace Poena.Core.Entity.Systems
{
    public class ContentSystem : ECSystem
    {
        public ContentSystem(SystemManager systemManager) : base(systemManager)
        {

        }

        public override void LoadContent(ContentManager contentManager)
        {
            List<ECEntity> entities =
                this.Manager.EntityManager.GetEntities(new Type[] {
                    typeof(SpriteComponent),
                    typeof(SkillComponent),
                });

            foreach (ECEntity ent in entities)
            {
                //Load the animations
                SpriteComponent anim = ent.GetComponent<SpriteComponent>();
                if (anim != null)
                {
                    anim.Animation.LoadContent(contentManager);
                }

                // TODO: Remove this and make it load what is necessary without calling directly to specific components
                //Load the icons
                SkillComponent skill = ent.GetComponent<SkillComponent>();
                if (skill != null)
                {
                    skill.HotBarTexture = contentManager.Load<Texture2D>(Variables.AssetPaths.UI_PATH + skill.HotBarTexturePath);
                }
            }
        }

        public override void Initiliaze()
        {
                
        }
    }
}
