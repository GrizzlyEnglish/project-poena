using Microsoft.Xna.Framework.Graphics;

namespace Poena.Core.Entity.Components
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
