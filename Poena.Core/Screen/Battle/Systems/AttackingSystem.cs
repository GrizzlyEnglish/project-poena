using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Poena.Core.Screen.Battle.Components;
using System;

namespace Poena.Core.Screen.Battle.Systems
{
    public class AttackingSystem : EntityUpdateSystem
    {
        private ComponentMapper<TileHighlightComponent> _tileHighlightMapper;
        private ComponentMapper<SkillComponent> _skillMapper;
        private ComponentMapper<AttackingComponent> _attackingMapper;

        public AttackingSystem() 
            : base(Aspect.All(typeof(AttackingComponent)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _skillMapper = mapperService.GetMapper<SkillComponent>();
            _tileHighlightMapper = mapperService.GetMapper<TileHighlightComponent>();
            _attackingMapper = mapperService.GetMapper<AttackingComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (int entityId in ActiveEntities)
            {

            }
        }
    }
}
