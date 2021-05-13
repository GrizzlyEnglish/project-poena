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
        private ComponentMapper<StatsComponent> _statsMapper;

        public TurnSystem() 
            : base(Aspect.One(typeof(TurnComponent),typeof(SelectedComponent)))
        {

        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _turnMapper = mapperService.GetMapper<TurnComponent>();
            _selectedMapper = mapperService.GetMapper<SelectedComponent>();
            _statsMapper = mapperService.GetMapper<StatsComponent>();
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
                StatsComponent stats = _statsMapper.Get(entityId);

                if (turn.TurnComplete)
                {
                    turn.CurrentTime = 0;
                    turn.TurnComplete = false;
                } 
                else
                {

                    //TODO: rce - Do we need to add logic to not update?
                    turn.CurrentTime += stats.GetTurnTick(gameTime.ElapsedGameTime.TotalSeconds);

                    // There turn is available
                    if (turn.CurrentTime >= turn.TimeForTurn)
                    {
                        // Make sure this doesn't overflow
                        turn.CurrentTime = turn.TimeForTurn;
                        break;
                    }

                }
            }

        }
    }
}
