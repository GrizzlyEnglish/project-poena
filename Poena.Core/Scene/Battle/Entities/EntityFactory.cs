using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Poena.Core.Scene.Battle.Board;
using Poena.Core.Common;
using Poena.Core.Entity.Components;
using Poena.Core.Entity;
using Poena.Core.Scene.Battle.Components;
using Poena.Core.Common.Enums;

namespace Poena.Core.Scene.Battle.Entities
{
    public static class EntityFactory
    {
        private static Dictionary<EntityTypeEnum, string[]> entity_sprite_mappings =
            new Dictionary<EntityTypeEnum, string[]>
        {
                { EntityTypeEnum.Debug, new string[] { Variables.AssetPaths.ENTITY_PATH + "/" + "icon_adventurer1" } },
                { EntityTypeEnum.DebugNPC, new string[] { Variables.AssetPaths.ENTITY_PATH + "/" + "icon_giantRat" } },
        };

        //TODO: rce - Remove everything below and load components and their data from a file.
        //            This file will just read the data and return a fully crafted entity

        public static ECEntity GenerateNPC()
        {
            ECEntity e = new ECEntity();

            //Add basics like position and render

            SpriteComponent anim = new SpriteComponent();
            anim.NewSprite(new Vector2(.5f, .8f), entity_sprite_mappings[EntityTypeEnum.DebugNPC]);

            PositionComponent position = new PositionComponent();
            position.tile_position = new BoardGridPosition(5, 5, 0).GetWorldAnchorPosition();
            
            StatsComponent stats = new StatsComponent();

            TurnComponent turn = new TurnComponent();
            turn.time_for_turn = 15;

            e.AddComponent(anim, position, turn, stats);

            //TODO: rce - Consider making this call a component factory

            return e;
        }

        public static ECEntity GenerateEntity()
        {
            ECEntity e = new ECEntity();

            //Add basics like position and render

            SpriteComponent anim = new SpriteComponent();
            anim.NewSprite(new Vector2(.5f, .7f), entity_sprite_mappings[EntityTypeEnum.Debug]);

            PositionComponent position = new PositionComponent();
            position.tile_position = new BoardGridPosition(9, 9, 0).GetWorldAnchorPosition();

            PlayerControllerComponent pcc = new PlayerControllerComponent();
            StatsComponent stats = new StatsComponent();

            TurnComponent turn = new TurnComponent();
            turn.time_for_turn = 15;

            SkillComponent skill = new SkillComponent() {
                HotBarTexturePath = "active1",
                Name = "Test Skill"
            };

            e.AddComponent(anim, position, pcc, turn, stats, skill);

            //TODO: rce - Consider making this call a component factory

            return e;
        }
    }
}
