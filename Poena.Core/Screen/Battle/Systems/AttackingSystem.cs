using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Poena.Core.Screen.Battle.Components;
using System;

namespace Poena.Core.Screen.Battle.Systems
{
    public class AttackingSystem : EntityUpdateSystem
    {
        public AttackingSystem() 
            : base(Aspect.All(typeof(AttackingComponent)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}
