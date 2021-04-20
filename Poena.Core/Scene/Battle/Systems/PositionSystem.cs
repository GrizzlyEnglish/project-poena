using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Poena.Core.Common;
using Poena.Core.Entity;
using Poena.Core.Entity.Components;
using Poena.Core.Entity.Managers;
using Poena.Core.Entity.Systems;
using Poena.Core.Events;
using Poena.Core.Extensions;
using Poena.Core.Scene.Battle.Board;

namespace Poena.Core.Scene.Battle.Systems
{
    public class PositionSystem : ECSystem
    {

        public PositionSystem(SystemManager systemManager) : base(systemManager)
        {

        }

        public override void Initiliaze()
        {
        }
        
        public override void Update(double dt)
        {
            Event evt = EventQueueHandler.GetInstance().GetEvent("battle_scene", "clicked_tile");

            //Get entities that have a position
            List<ECEntity> entities =
                this.Manager.EntityManager
                .GetEntities(new Type[] { typeof(PositionComponent), typeof(MovementComponent) });

            foreach (ECEntity ent in entities)
            {
                PositionComponent pos = ent.GetComponent<PositionComponent>();
                MovementComponent movement = ent.GetComponent<MovementComponent>();
                SelectedComponent selected = ent.GetComponent<SelectedComponent>();

                //Check if the entity is moving
                if (movement != null && movement.path_to_destination.Count > 0)
                {
                    //LERP to position
                    Vector2 destination = movement.path_to_destination.Peek();
                    pos.tile_position = pos.tile_position.Lerp(destination, (float)dt * 3.5f);

                    if (pos.tile_position.Distance(destination) < 5)
                    {
                        Vector2 last_pos = movement.path_to_destination.Dequeue();

                        if (movement.path_to_destination.Count == 0)
                        {
                            //Entity is finished moving notify turn system to reset
                            this.Manager.Message("end_turn", ent);
                            pos.tile_position = last_pos;
                            ent.RemoveComponent(movement);
                        }
                    }
                }
                //Check if a tile has been clicked
                else if (evt != null && selected != null)
                {
                    //Get the destination tile
                    BoardTile destination_tile = (BoardTile)evt.data;
                    BoardGrid bg = destination_tile.board_grid;
                    BoardTile on_tile = bg[Coordinates.WorldToBoard(pos.tile_position)];
                    Vector2 destination_tile_anchor = destination_tile.position.GetWorldAnchorPosition();

                    //TODO: rce - Add logic to make sure tile is moveable
                    bool isValid = selected.possible_positions.Contains(destination_tile_anchor);

                    // This is likely a deslection if the same tile
                    if (!on_tile.IsEqual(destination_tile) && isValid)
                    {
                        //Setup the movement component and append to the component
                        movement = new MovementComponent();
                        ent.AddComponent(movement);
                        
                        //Flag the event it was digested
                        evt.HandleEvent();

                        //We need to determine the path that entity will take
                        List<BoardTile> path = bg.ShortestPath(on_tile, destination_tile);

                        //Add the anchor positions to the queue of the path
                        foreach (BoardTile bt in path)
                        {
                            movement.path_to_destination.Enqueue(bt.position.GetWorldAnchorPosition());
                        }
                    }
                }
            }
        }
    }
}

