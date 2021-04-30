using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using Poena.Core.Screen.Battle.Board;
using Poena.Core.Common;
using Poena.Core.Entity.Components;
using Poena.Core.Screen.Battle.Components;
using Poena.Core.Common.Enums;

namespace Poena.Core.Screen.Battle.Entities
{
    public static class EntityFactory
    {
        private static Dictionary<EntityTypeEnum, string[]> entity_sprite_mappings =
            new Dictionary<EntityTypeEnum, string[]>
        {
                { EntityTypeEnum.Debug, new string[] { Assets.GetEntity(EntityType.Adventurer) } },
                { EntityTypeEnum.DebugNPC, new string[] { Assets.GetEntity(EntityType.GiantRat) } },
        };

        //TODO: rce - Remove everything below and load components and their data from a file.
        //            This file will just read the data and return a fully crafted entity

        public static void GenerateNPC(World world)
        {
            var entity = world.CreateEntity();

            SpriteComponent anim = new SpriteComponent();
            anim.NewSprite(new Vector2(.5f, .8f), entity_sprite_mappings[EntityTypeEnum.DebugNPC]);
            entity.Attach(anim);

            PositionComponent position = new PositionComponent();
            position.tile_position = new BoardGridPosition(5, 5, 0).GetWorldAnchorPosition();
            entity.Attach(position);
            
            StatsComponent stats = new StatsComponent();
            entity.Attach(stats);

            TurnComponent turn = new TurnComponent();
            turn.time_for_turn = 15;
            entity.Attach(turn);
        }

        public static void GenerateEntity(World world)
        {
            var entity = world.CreateEntity();

            SpriteComponent anim = new SpriteComponent();
            anim.NewSprite(new Vector2(.5f, .7f), entity_sprite_mappings[EntityTypeEnum.Debug]);
            entity.Attach(anim);

            PositionComponent position = new PositionComponent();
            position.tile_position = new BoardGridPosition(9, 9, 0).GetWorldAnchorPosition();
            entity.Attach(position);

            PlayerControllerComponent pcc = new PlayerControllerComponent();
            entity.Attach(pcc);

            StatsComponent stats = new StatsComponent();
            entity.Attach(stats);

            TurnComponent turn = new TurnComponent();
            turn.time_for_turn = 15;
            entity.Attach(turn);

            SkillComponent skill = new SkillComponent() {
                HotBarTexturePath = "active1",
                Name = "Test Skill"
            };
            entity.Attach(skill);
        }
    }
}
