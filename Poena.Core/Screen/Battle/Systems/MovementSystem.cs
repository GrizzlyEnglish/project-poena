using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Poena.Core.Extensions;
using Poena.Core.Screen.Battle.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poena.Core.Screen.Battle.Systems
{
    public class MovementSystem : EntityUpdateSystem
    {
        private ComponentMapper<MovementComponent> _movementMapper;
        private ComponentMapper<PositionComponent> _positionMapper;

        public MovementSystem() :
            base(Aspect.All(typeof(MovementComponent)))
        {

        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _positionMapper = mapperService.GetMapper<PositionComponent>();
            _movementMapper = mapperService.GetMapper<MovementComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (int entityId in ActiveEntities)
            {
                PositionComponent pos = _positionMapper.Get(entityId);
                MovementComponent movement = _movementMapper.Get(entityId);

                // LERP to position
                Vector2 destination = movement.PathToDestination.Peek();
                pos.TilePosition = pos.TilePosition.Lerp(destination, (float)(gameTime.ElapsedGameTime.TotalSeconds * 3.5f));

                if (pos.TilePosition.Distance(destination) < 5)
                {
                    Vector2 last_pos = movement.PathToDestination.Dequeue();

                    if (movement.PathToDestination.Count == 0)
                    {
                        // Entity is finished moving notify turn system to reset
                        pos.TilePosition = last_pos;
                        Entity ent = this.GetEntity(entityId);
                        ent.Detach<MovementComponent>();
                        ent.Detach<TileHighlightComponent>();
                        ent.Detach<SelectedComponent>();

                        TurnComponent turnComponent = ent.Get<TurnComponent>();
                        turnComponent.TurnComplete = true;
                    }
                }
            }
        }
    }
}
