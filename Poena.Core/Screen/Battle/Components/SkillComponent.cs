using Microsoft.Xna.Framework.Graphics;
using Poena.Core.Common.Enums;

namespace Poena.Core.Screen.Battle.Components
{
    public class SkillComponent
    {
        public AttackType AttackType { get; private set; } = AttackType.Skill;
        public TilePattern AttackPattern { get; set; }
        public int Distance { get; set; }
        public string Name { get; set; }
        public string HotBarTexturePath { get; set; }
    }
}