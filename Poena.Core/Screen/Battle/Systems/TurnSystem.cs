using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Poena.Core.Screen.Battle.Components;

namespace Poena.Core.Screen.Battle.Systems
{
    public class TurnSystem : EntityUpdateSystem
    {
        private ComponentMapper<TurnComponent> _turnMapper;
        private ComponentMapper<SelectedComponent> _selectedMapper;

        public TurnSystem() 
            : base(Aspect.One(typeof(TurnComponent),typeof(SelectedComponent)))
        {

        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _turnMapper = mapperService.GetMapper<TurnComponent>();
            _selectedMapper = mapperService.GetMapper<SelectedComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (int entityId in ActiveEntities)
            {
                if (_selectedMapper.Has(entityId))
                {
                    // We have a selected entity don't update ticker for people
                    return;
                }
            }

            foreach (int entityId in ActiveEntities)
            {
                TurnComponent turn = _turnMapper.Get(entityId);

                //TODO: rce - Do we need to add logic to not update?
                turn.current_time += gameTime.ElapsedGameTime.TotalSeconds;

                // There turn is available
                if (turn.current_time >= turn.time_for_turn)
                {
                    // Make sure this doesn't overflow
                    turn.current_time = turn.time_for_turn;
                    break;
                }

            }

        }
    }
}
