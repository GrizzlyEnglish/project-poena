using Microsoft.Xna.Framework;
using Poena.Core.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poena.Core.Screen.Battle.Components
{
    public class TileHighlightComponent
    {
        public TileHighlight TileHighlight { get; set; }
        public AttackType? AttackType { get; set; }
        public List<Vector2> PossiblePositions { get; set; }
        public Vector2 CheckPosition { get; set; }
        public List<Vector2> HighlightPositions { get; set; }
        public bool HighlightCheck { get { return HighlightPositions != null; } }
    }
}
