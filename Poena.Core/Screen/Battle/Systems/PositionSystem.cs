using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Poena.Core.Common;
using Poena.Core.Extensions;
using Poena.Core.Screen.Battle.Board;
using Poena.Core.Screen.Battle.Components;

namespace Poena.Core.Screen.Battle.Systems
{
    public class PositionSystem : EntityUpdateSystem
    {
        private BoardGrid BoardGrid;
        private ComponentMapper<PositionComponent> _positionMapper;
        private ComponentMapper<MovementComponent> _movementMapper;
        private ComponentMapper<SelectedComponent> _selectedMapper;
        private ComponentMapper<AttackingComponent> _attackingMapper;

        public PositionSystem(BoardGrid boardGrid) : 
            base(Aspect.All(typeof(PositionComponent)))
        {
            this.BoardGrid = boardGrid;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _attackingMapper = mapperService.GetMapper<AttackingComponent>();
            _positionMapper = mapperService.GetMapper<PositionComponent>();
            _selectedMapper = mapperService.GetMapper<SelectedComponent>();
            _movementMapper = mapperService.GetMapper<MovementComponent>();
        }
        
        public override void Update(GameTime gameTime)
        {
            // TODO: Handle clicked tile
            BoardTile selectedTile = null;

            foreach (int entityId in ActiveEntities)
            {
                MovementComponent movement = _movementMapper.Get(entityId);
                PositionComponent pos = _positionMapper.Get(entityId);
                SelectedComponent selected = _selectedMapper.Get(entityId);

                // Check if the entity is moving
                if (movement != null && movement.path_to_destination.Count > 0)
                {
                    // LERP to position
                    Vector2 destination = movement.path_to_destination.Peek();
                    pos.TilePosition = pos.TilePosition.Lerp(destination, (float)(gameTime.ElapsedGameTime.TotalMilliseconds * 3.5f));

                    if (pos.TilePosition.Distance(destination) < 5)
                    {
                        Vector2 last_pos = movement.path_to_destination.Dequeue();

                        if (movement.path_to_destination.Count == 0)
                        {
                            // Entity is finished moving notify turn system to reset
                            pos.TilePosition = last_pos;
                            _movementMapper.Delete(entityId);
                        }
                    }
                }
                //Check if a tile has been clicked
                else if (selectedTile != null && selected != null && !_attackingMapper.Has(entityId))
                {
                    BoardGrid bg = selectedTile.BoardGrid;
                    BoardTile on_tile = bg[Coordinates.WorldToBoard(pos.TilePosition)];
                    Vector2 destination_tile_anchor = selectedTile.Position.GetWorldAnchorPosition();

                    //TODO: rce - Add logic to make sure tile is moveable
                    bool isValid = selected.possible_positions.Contains(destination_tile_anchor);

                    // This is likely a deslection if the same tile
                    if (!on_tile.IsEqual(selectedTile) && isValid)
                    {
                        // Mark tile as used
                        // TODO: Handle clicked tile

                        // Setup the movement component and append to the component
                        movement = new MovementComponent();
                        _movementMapper.Put(entityId, movement);
                        
                        // We need to determine the path that entity will take
                        List<BoardTile> path = bg.ShortestPath(on_tile, selectedTile);

                        // Add the anchor positions to the queue of the path
                        foreach (BoardTile bt in path)
                        {
                            movement.path_to_destination.Enqueue(bt.Position.GetWorldAnchorPosition());
                        }
                    }
                }
            }
        }
    }
}

