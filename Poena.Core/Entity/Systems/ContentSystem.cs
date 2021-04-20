using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Poena.Core.Common;
using Poena.Core.Entity.Components;
using Poena.Core.Entity.Managers;

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
                    anim.animation.LoadContent(contentManager);
                }

                //Load the icons
                SkillComponent skill = ent.GetComponent<SkillComponent>();
                if (skill != null)
                {
                    skill.skill_icon = contentManager.Load<Texture2D>(Variables.AssetPaths.UI_PATH + skill.skill_path);
                }
            }
        }

        public override void Initiliaze()
        {
                
        }
    }
}
