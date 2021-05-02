using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Poena.Core.Screen.Battle.Components
{
    public class SelectedComponent
    {
        public bool disadvantaged { get; set; }
        public List<Vector2> possible_positions { get; set; } = new List<Vector2>();
    }
}
