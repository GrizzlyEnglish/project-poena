using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Poena.Core.Entity.Components;

namespace Poena.Core.Screen.Battle.Components
{
    public class SelectedComponent : IComponent
    {
        public bool disadvantaged { get; set; }
        public List<Vector2> possible_positions { get; set; }

        public void Initialize()
        {
            possible_positions = new List<Vector2>();
        }
    }
}
