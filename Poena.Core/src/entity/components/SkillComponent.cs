using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project_Poena.Entity.Components
{
    public class SkillComponent : Component
    {
        public string skill_name;

        public string skill_path { get { return skill_name + "_icon"; } }

        public Texture2D skill_icon;

        public override void Initialize()
        {
            
        }
    }
}
