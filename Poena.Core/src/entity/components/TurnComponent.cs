using System;
using System.Collections.Generic;
using System.Text;

namespace Project_Poena.Entity.Components
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
