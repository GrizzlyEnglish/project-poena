﻿using Poena.Core.Entity.Components;

namespace Poena.Core.Scene.Battle.Components
{
    public class TurnComponent : Component
    {

        public double current_time { get; set; }
        public double time_for_turn { get; set; }

        public bool ready_for_turn { get { return current_time == time_for_turn;  } }

        public override void Initialize()
        {
            
        }
    }
}