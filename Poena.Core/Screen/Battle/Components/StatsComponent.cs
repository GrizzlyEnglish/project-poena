using System;

namespace Poena.Core.Screen.Battle.Components
{
    public class StatsComponent
    {

        public StatsComponent()
        {
            this.Stamina = 1;
            this.Strength = 1;
            this.Luck = 1;
            this.Agility = 1;
        }

        public int Strength { get; set; }

        public int Agility { get; set; }
        
        public int Stamina { get; set; }

        public int Luck { get; set; }

        public int GetMovementDistance()
        {
            return Math.Max(1, (int)Math.Round(Stamina / 3f)) + 2;
        }

        public double GetTurnTick(double tick)
        {
            if (Stamina < 3)
            {
                return tick + (tick / 6);
            } 
            else if (Stamina < 6)
            {
                return tick + (tick / 3);
            }
            else
            {
                return tick + tick;
            }
        }
    }
}
