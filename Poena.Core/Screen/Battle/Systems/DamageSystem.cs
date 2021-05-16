using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Poena.Core.Screen.Battle.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poena.Core.Screen.Battle.Systems
{
    public class DamageSystem : EntityUpdateSystem
    {
        private readonly BoardInteractionSystem _boardInteractionSystem;

        private ComponentMapper<DamageComponent> _damageMapper;
        private ComponentMapper<SpriteComponent> _spriteMapper;
        private ComponentMapper<HealthComponent> _healthMapper;
        private ComponentMapper<TurnComponent> _turnMapper;

        public DamageSystem(BoardInteractionSystem boardSystem)
            : base(Aspect.All(typeof(DamageComponent)))
        {
            _boardInteractionSystem = boardSystem;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _damageMapper = mapperService.GetMapper<DamageComponent>();
            _spriteMapper = mapperService.GetMapper<SpriteComponent>();
            _healthMapper = mapperService.GetMapper<HealthComponent>();
            _turnMapper = mapperService.GetMapper<TurnComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach(int entityId in ActiveEntities)
            {
                SpriteComponent sprite = _spriteMapper.Get(entityId);
                DamageComponent damage = _damageMapper.Get(entityId);
                damage.CurrentTime += gameTime.ElapsedGameTime.TotalSeconds;
                damage.OverallTime += gameTime.ElapsedGameTime.TotalSeconds;

                if (damage.CurrentTime >= .2)
                {
                    sprite.IsVisible = !sprite.IsVisible;
                    damage.CurrentTime = 0;
                }

                if (damage.OverallTime >= 1.5)
                {
                    sprite.IsVisible = true;
                    HealthComponent health = _healthMapper.Get(entityId);
                    health.Health -= damage.Damage;
                    if (health.Health <= 0) this.DestroyEntity(entityId);
                    _boardInteractionSystem.SelectedEntityEndTurn();
                    _damageMapper.Delete(entityId);
                }
            }
        }
    }
}
