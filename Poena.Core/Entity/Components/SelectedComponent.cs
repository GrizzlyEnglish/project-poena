using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Poena.Core.Entity.Components
{
    public class SelectedComponent : Component
    {
        //Flag for selected entity

        /*
         * Flag for entity not fully prepared for action
         * Disadvantages the turn
         */
        public bool disadvantaged { get; set; }

        /*
         * Possible positions to move to when selected
         */
        public List<Vector2> possible_positions { get; set; }

        public override void Initialize()
        {
            possible_positions = new List<Vector2>();
        }
    }
}
