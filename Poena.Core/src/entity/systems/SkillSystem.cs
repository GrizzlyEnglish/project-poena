using System;
using System.Collections.Generic;
using System.Text;
using Project_Poena.Entity.Managers;

namespace Project_Poena.Entity.Systems
{
    public class SkillSystem : ECSystem
    {
        public SkillSystem(SystemManager systemManager) : base(systemManager)
        {

        }

        public override bool RecieveMessage(string message, object data)
        {
            if (message == "entity_skill")
            {
                //TODO: Start adding skills
                return true;
            }

            return false;
        }

        public override void Initiliaze()
        {
            
        }
    }
}
