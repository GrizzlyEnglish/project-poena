using Poena.Core.Entity.Managers;
using Poena.Core.Entity.Systems;

namespace Poena.Core.Scene.Battle.Systems
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
