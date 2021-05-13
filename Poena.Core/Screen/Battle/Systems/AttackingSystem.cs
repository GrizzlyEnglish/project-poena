using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Poena.Core.Screen.Battle.Components;
using System;

namespace Poena.Core.Screen.Battle.Systems
{
    public class AttackingSystem : EntityUpdateSystem
    {
        private ComponentMapper<AttackingComponent> _attackingMapper;
        private ComponentMapper<HealthComponent> _healthMapper;

        private readonly BoardInteractionSystem _boardSystem;

        public AttackingSystem(BoardInteractionSystem boardSystem) 
            : base(Aspect.All(typeof(AttackingComponent)))
        {
            _boardSystem = boardSystem;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _attackingMapper = mapperService.GetMapper<AttackingComponent>();
            _healthMapper = mapperService.GetMapper<HealthComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (int entityId in ActiveEntities)
            {
                AttackingComponent attackingComponent = _attackingMapper.Get(entityId);
                HealthComponent healthComponent = _healthMapper.Get(attackingComponent.AttackingEntityId);

                healthComponent.Health -= 100;

                if (healthComponent.Health <= 0)
                {
                    this.DestroyEntity(attackingComponent.AttackingEntityId);

                    _boardSystem.DeselectEntity(entityId);
                }
            }
        }
    }
}
