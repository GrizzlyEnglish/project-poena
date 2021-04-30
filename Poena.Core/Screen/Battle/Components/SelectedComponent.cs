using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Poena.Core.Entity.Components;

namespace Poena.Core.Screen.Battle.Components
{
    public class SelectedComponent : IComponent
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

        public void Initialize()
        {
            possible_positions = new List<Vector2>();
        }
    }
}
