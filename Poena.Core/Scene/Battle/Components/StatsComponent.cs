using System;
using Poena.Core.Entity.Components;

namespace Poena.Core.Scene.Battle.Components
{
    public class StatsComponent : Component
    {

        /*
         * Speed:
         *      Increases the speed of the 'turn bar'
         *      Increases chance to dodge attacks
         *      ** Scales with Dexterity **
         */
        public int speed { get; set; }

        /*
         * Endurance:
         *      Increases the distance the entity can move on a turn
         *      Increases ability to allow additional actions
         *      ** Scales with strength **
         */
        public int endurance { get; set; }

        /*
         * Strenth:
         *      Increases attack with short range weapons
         */
        public int strength { get; set; }

        /*
         * Dexterity:
         *      Increases attack with long range weapons
         */
        public int dexterity { get; set; }

        /*
         * Defense:
         *      Decreases damage taken without armor
         */
        public int defense { get; set; }

        /*
         * Luck:
         *      Increases critical attack
         */
        public int luck { get; set; }
        
        /*
         * Level:
         *      Increases stats by base amount
         */
        public int current_level { get; set; }

        /*
         * Experience:
         *      Amount of current experience
         */
        public int experience { get; set; }

        /*
         * Experience to level:
         *      Amount of experience needed to increase level
         */
        public int experience_to_level { get; set; }

        public int GetMovementDistance(bool disadvantaged = false)
        {
            int min_distance = 5;
            int max_distance = 10;

            int distance = (min_distance + ((this.speed + this.dexterity) / 2)) / (disadvantaged ? 2 : 1);

            return Math.Min(distance, max_distance);
        }

        public override void Initialize()
        {
            //TODO: rce - Move this to generator
            this.current_level = 1;
            this.speed = 1;
            this.dexterity = 1;
            this.endurance = 1;
            this.defense = 1;
            this.experience_to_level = 100;
        }
    }
}
