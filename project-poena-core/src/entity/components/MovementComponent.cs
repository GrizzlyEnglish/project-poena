﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project_Poena.Entity.Components
{
    public class MovementComponent : Component
    {
        //Set when tile is selected for movement
        public Queue<Vector2> path_to_destination { get; set; }
        public float delta_offset { get; set; }

        public override void Initialize()
        {
            path_to_destination = new Queue<Vector2>();
            if (delta_offset == 0) delta_offset = 3.5f;
        }
    }
}
