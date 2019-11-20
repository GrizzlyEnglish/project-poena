using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Project_Poena.Common.Variables;
using Project_Poena.Entity.Managers;
using Project_Poena.Entity.Components;
using Project_Poena.Entity.Entities;

namespace Project_Poena.Entity.Systems
{
    public class ContentSystem : ECSystem
    {
        public ContentSystem(SystemManager systemManager) : base(systemManager)
        {

        }

        public override void LoadContent(ContentManager contentManager)
        {
            List<ECEntity> entities =
                this.manager.entity_manager.GetEntities(new Type[] {
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
