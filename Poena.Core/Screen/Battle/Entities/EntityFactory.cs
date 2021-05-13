using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using Poena.Core.Screen.Battle.Board;
using Poena.Core.Common;
using Poena.Core.Screen.Battle.Components;
using Poena.Core.Common.Enums;
using Poena.Core.Managers;

namespace Poena.Core.Screen.Battle.Entities
{
    public class EntityFactory
    {
        private readonly AssetManager _assetManager;

        public EntityFactory(AssetManager assetManager)
        {
            _assetManager = assetManager;
        }

        //TODO: rce - Remove everything below and load components and their data from a file.
        //            This file will just read the data and return a fully crafted entity

        public void GenerateNPC(World world)
        {
            var entity = world.CreateEntity();

            SpriteComponent anim = new SpriteComponent();
            anim.Texture = _assetManager.GetTexture(Assets.GetEntity(EntityType.GiantRat));
            anim.AnchorOffset = new Vector2(.5f, .8f);
            entity.Attach(anim);

            PositionComponent position = new PositionComponent();
            position.TilePosition = new BoardGridPosition(5, 5, 0).GetWorldAnchorPosition();
            entity.Attach(position);
            
            StatsComponent stats = new StatsComponent();
            entity.Attach(stats);

            HealthComponent health = new HealthComponent();
            health.Health = 15;
            entity.Attach(health);

            TurnComponent turn = new TurnComponent();
            turn.TimeForTurn = 15;
            entity.Attach(turn);
        }

        public void GenerateEntity(World world)
        {
            var entity = world.CreateEntity();

            SpriteComponent anim = new SpriteComponent();
            anim.Texture = _assetManager.GetTexture(Assets.GetEntity(EntityType.Adventurer));
            anim.AnchorOffset = new Vector2(.5f, .7f);
            entity.Attach(anim);

            PositionComponent position = new PositionComponent();
            position.TilePosition = new BoardGridPosition(9, 9, 0).GetWorldAnchorPosition();
            entity.Attach(position);

            PlayerControllerComponent pcc = new PlayerControllerComponent();
            entity.Attach(pcc);

            StatsComponent stats = new StatsComponent();
            entity.Attach(stats);

            TurnComponent turn = new TurnComponent();
            turn.TimeForTurn = 15;
            entity.Attach(turn);

            HealthComponent health = new HealthComponent();
            health.Health = 15;
            entity.Attach(health);

            SkillComponent skill = new SkillComponent() {
                HotBarTexturePath = "active1",
                Name = "Test Skill",
                AttackPattern = TilePattern.Flood,
                Distance = 2
            };
            entity.Attach(skill);
        }
    }
}
