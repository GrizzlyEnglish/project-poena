using System;
using System.Collections.Generic;
using System.Linq;
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
        private ComponentMapper<PositionComponent> _positionMapper;
        private ComponentMapper<MovementComponent> _movementMapper;
        private ComponentMapper<SelectedComponent> _selectedMapper;
        private ComponentMapper<AttackingComponent> _attackingMapper;
        private ComponentMapper<TileHighlightComponent> _tileHighlightMapper;
        private ComponentMapper<TurnComponent> _turnMapper;

        private readonly BoardGrid _boardGrid;

        public PositionSystem(BoardGrid boardGrid) : 
            base(Aspect.One(typeof(PositionComponent)))
        {
            _boardGrid = boardGrid;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _attackingMapper = mapperService.GetMapper<AttackingComponent>();
            _positionMapper = mapperService.GetMapper<PositionComponent>();
            _selectedMapper = mapperService.GetMapper<SelectedComponent>();
            _movementMapper = mapperService.GetMapper<MovementComponent>();
            _turnMapper = mapperService.GetMapper<TurnComponent>();
            _tileHighlightMapper = mapperService.GetMapper<TileHighlightComponent>();
        }
        
        public override void Update(GameTime gameTime)
        {
            BoardTile selectedTile = _boardGrid.GetSelectedTile();

            foreach (int entityId in ActiveEntities)
            {
                MovementComponent movement = _movementMapper.Get(entityId);
                PositionComponent pos = _positionMapper.Get(entityId);
                SelectedComponent selected = _selectedMapper.Get(entityId);

                // Check if the entity is moving
                if (movement != null && movement.PathToDestination.Count > 0)
                {
                    // LERP to position
                    Vector2 destination = movement.PathToDestination.Peek();
                    pos.TilePosition = pos.TilePosition.Lerp(destination, (float)(gameTime.ElapsedGameTime.TotalSeconds * 3.5f));

                    if (pos.TilePosition.Distance(destination) < 5)
                    {
                        Vector2 last_pos = movement.PathToDestination.Dequeue();

                        if (movement.PathToDestination.Count == 0)
                        {
                            // Entity is finished moving notify turn system to reset
                            pos.TilePosition = last_pos;
                            _movementMapper.Delete(entityId);
                            _selectedMapper.Delete(entityId);
                            _turnMapper.Get(entityId).EndTurn();
                            _tileHighlightMapper.Delete(entityId);
                        }
                    }
                }
                //Check if a tile has been clicked
                else if (selectedTile != null && selected != null && !_attackingMapper.Has(entityId))
                {
                    BoardTile onTile = _boardGrid[Coordinates.WorldToBoard(pos.TilePosition)];
                    Vector2 destTileAnchor = selectedTile.Position.GetWorldAnchorPosition();

                    //TODO: rce - Add logic to make sure tile is moveable
                    TileHighlightComponent highlight = _tileHighlightMapper.Get(entityId);
                    bool isValid = highlight.TilePositions.Contains(destTileAnchor);

                    // This is likely a deslection if the same tile
                    if (!onTile.IsEqual(selectedTile) && isValid)
                    {
                        // Mark tile as used
                        _boardGrid.ClearSelectedTile();

                        // Setup the movement component and append to the component
                        movement = new MovementComponent();
                        _movementMapper.Put(entityId, movement);
                        
                        // We need to determine the path that entity will take
                        List<BoardTile> path = _boardGrid.ShortestPath(onTile, selectedTile);

                        // Add the anchor positions to the queue of the path
                        foreach (BoardTile bt in path)
                        {
                            movement.PathToDestination.Enqueue(bt.Position.GetWorldAnchorPosition());
                        }

                        // Highlight the path taken
                        _tileHighlightMapper.Get(entityId).TilePositions = movement.PathToDestination.ToList();
                    }
                }
            }
        }
    }
}

