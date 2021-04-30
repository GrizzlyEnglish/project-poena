using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Poena.Core.Entity.Components;

namespace Poena.Core.Screen.Battle.Components
{
    public class MovementComponent : IComponent
    {
        //Set when tile is selected for movement
        public Queue<Vector2> path_to_destination { get; set; }
        public float delta_offset { get; set; }

        public void Initialize()
        {
            path_to_destination = new Queue<Vector2>();
            if (delta_offset == 0) delta_offset = 3.5f;
        }
    }
}
