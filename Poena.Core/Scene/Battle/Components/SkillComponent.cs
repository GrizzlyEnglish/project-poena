using Microsoft.Xna.Framework.Graphics;
using Poena.Core.Common.Enums;
using Poena.Core.Entity.Components;

namespace Poena.Core.Scene.Battle.Components
{
    public class SkillComponent : Component
    {
        public AttackTypeEnum AttackType { get; private set; }

        public Texture2D HotBarTexture { get; set; }

        public string Name { get; set; }

        public string HotBarTexturePath { get; set; }

        public override void Initialize()
        {
            AttackType = AttackTypeEnum.Skill;
        }
    }
}