using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Poena.Core.Screen.Battle.Components
{
    public class MovementComponent
    {
        // Set when tile is selected for movement
        public Queue<Vector2> PathToDestination { get; set; } = new Queue<Vector2>();
        public bool IsMoving { get; set; }
    }
}
