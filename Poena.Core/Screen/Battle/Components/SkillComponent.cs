using Microsoft.Xna.Framework.Graphics;
using Poena.Core.Common.Enums;
using Poena.Core.Entity.Components;

namespace Poena.Core.Screen.Battle.Components
{
    public class SkillComponent : IComponent
    {
        public AttackType AttackType { get; private set; }

        public Texture2D HotBarTexture { get; set; }

        public string Name { get; set; }

        public string HotBarTexturePath { get; set; }

        public void Initialize()
        {
            AttackType = AttackType.Skill;
        }
    }
}