using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Poena.Core.Common;
using Poena.Core.Entity;
using Poena.Core.Entity.Components;
using Poena.Core.Entity.Managers;
using Poena.Core.Entity.Systems;
using Poena.Core.Events;
using Poena.Core.Scene.Battle.Board;
using Poena.Core.Scene.Battle.Layers;

namespace Poena.Core.Scene.Battle.Systems
{
    public class SelectionSystem : ECSystem
    {

        public SelectionSystem(SystemManager systemManager) : base(systemManager)
        {
        }

        public override void Initiliaze()
        {
        }

        public override bool RecieveMessage(string message, object data)
        {
            if (message == "select_entity")
            {
                ECEntity ent = (ECEntity)data;
                PositionComponent pos = ent.GetComponent<PositionComponent>();

                //TODO: rce - Determine how we are going to get things without direct connection
                BattleBoard board = this.manager.SceneLayer.CurrentScene.GetSceneLayer<BattleEntityLayer>().GetLayerNode<BattleBoard>();
                BoardTile on_tile = board.grid[Coordinates.WorldToBoard(pos.tile_position)];

                this.SelectEntity(ent, on_tile);

                return true;
            }

            return false;
        }

        public override void Update(double dt)
        {
            Event evt = EventQueueHandler.GetInstance().GetEvent("battle_scene", "clicked_tile");

            //If we have an event lets check if we seleced an entity
            if (evt != null)
            {
                //Get the tile
                BoardTile selected_tile = (BoardTile)evt.data;

                //See if somone is already selected
                ECEntity selected_ent = this.manager.EntityManager.GetEntity(typeof(SelectedComponent));
                
                //If we have a selected entity, if not current turn deselect them otherwise ignore
                if (selected_ent != null)
                {
                    TurnComponent turn = selected_ent.GetComponent<TurnComponent>();
                    PositionComponent pos = selected_ent.GetComponent<PositionComponent>();
                    //If they are deslect them
                    if (!turn.ready_for_turn && selected_tile.position.GetWorldAnchorPosition() == pos.tile_position)
                    {
                        selected_ent.RemoveComponent(typeof(SelectedComponent));
                    }
                    else
                    {
                    }

                    return;
                }
                
                List<ECEntity> entities =
                    this.manager.EntityManager
                        .GetEntities(true, new Type[] { typeof(PositionComponent), typeof(PlayerControllerComponent) });

                foreach (ECEntity ent in entities)
                {
                    PositionComponent pos = ent.GetComponent<PositionComponent>();

                    //Check if the entities anchor point is inside the tile
                    if (selected_tile.position.GetWorldAnchorPosition() == pos.tile_position)
                    {
                        this.SelectEntity(ent, selected_tile, evt);
                        break;
                    }
                }
            }
        }

        private void SelectEntity(ECEntity ent, BoardTile selected_tile, Event evt = null)
        {
            //Get the componenets
            TurnComponent turn = ent.GetComponent<TurnComponent>();
            StatsComponent stats = ent.GetComponent<StatsComponent>();
            //Created the selected
            SelectedComponent selected = new SelectedComponent();
            //If not ready for turn disadvantage entity
            selected.disadvantaged = !turn.ready_for_turn;
            //This entity was selected
            ent.AddComponent(selected);
            //Digest the event if we have one
            if (evt !=null) evt.HandleEvent();
            //Finally get all the possible tile anchors
            List<Vector2> tiles =
                selected_tile.board_grid.Flood(selected_tile, stats.GetMovementDistance(selected.disadvantaged))
                .Select(bt => bt.position.GetWorldAnchorPosition()).ToList();
            //Filter blocked tiles
            List<Vector2> ent_positions = this.manager.EntityManager
                .GetEntities(typeof(PositionComponent))
                .Select(e => e.GetComponent<PositionComponent>().tile_position)
                .ToList();
            tiles.RemoveAll(t => ent_positions.Contains(t));
            //Set movements possible movement tiles
            selected.possible_positions = tiles;
            //Move camera to entity
            this.manager.SceneLayer.CurrentScene.GetSceneLayer<BattleEntityLayer>().MoveCamera(selected_tile.position.GetWorldAnchorPosition());
            //Tell the event channel someone was selected
            EventQueueHandler.GetInstance().QueueEvent(new Event("entity", "selected", ent));
        }
        
    }
}
