using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Poena.Core.Common;
using Poena.Core.Common.Enums;
using Poena.Core.Screen.Battle.Board;
using Poena.Core.Screen.Battle.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poena.Core.Screen.Battle.Systems
{
    public class BoardInteractionSystem : EntityUpdateSystem
    {
        private readonly Battle _battle;
        private readonly BoardGrid _boardGrid;

        private ComponentMapper<SelectedComponent> _selectedMapper;
        private ComponentMapper<PositionComponent> _positionMapper;
        private ComponentMapper<AttackingComponent> _attackingMapper;
        private ComponentMapper<TileHighlightComponent> _tileHighlightMapper;
        private ComponentMapper<TurnComponent> _turnMapper;
        private ComponentMapper<StatsComponent> _statsMapper;
        private ComponentMapper<MovementComponent> _movementMapper;

        public BoardInteractionSystem(Battle battle, BoardGrid boardGrid)
            : base(Aspect.All())
        {
            _battle = battle;
            _boardGrid = boardGrid;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _selectedMapper = mapperService.GetMapper<SelectedComponent>();
            _positionMapper = mapperService.GetMapper<PositionComponent>();
            _statsMapper = mapperService.GetMapper<StatsComponent>();
            _turnMapper = mapperService.GetMapper<TurnComponent>();
            _tileHighlightMapper = mapperService.GetMapper<TileHighlightComponent>();
            _movementMapper = mapperService.GetMapper<MovementComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            BoardTile selectedTile = _boardGrid.GetSelectedTile();

            if (selectedTile == null)
            {
                // Nothing to do
                return;
            }

            int? selectedEntityId = null;
            int? entityOnTileId = null;

            foreach (int entityId in ActiveEntities)
            {
                if (_selectedMapper.Has(entityId))
                {
                    selectedEntityId = entityId;
                }

                PositionComponent pos = _positionMapper.Get(entityId);
                if (pos.TilePosition == selectedTile.Position.GetWorldAnchorPosition())
                {
                    entityOnTileId = entityId;
                }
            }

            // Working backwards we need to determine how to interact

            // Tile is clicked to attack the entity on tile, must have entity on tile and current selected in attacking
            if (entityOnTileId.HasValue && selectedEntityId.HasValue && _tileHighlightMapper.Get(selectedEntityId.Value)?.TileHighlight == TileHighlight.Attack)
            {
                //AttackEntity();
            }

            // Tile is clicked to move to tile
            else if (!entityOnTileId.HasValue && selectedEntityId.HasValue && _tileHighlightMapper.Get(selectedEntityId.Value)?.TileHighlight == TileHighlight.Movement)
            {
                TileHighlightComponent tileHighlight = _tileHighlightMapper.Get(selectedEntityId.Value);

                if (!tileHighlight.HighlightCheck)
                {
                    GetPathOfMovement(selectedEntityId.Value, tileHighlight, selectedTile);
                }
                else if (tileHighlight.CheckPosition != selectedTile.Position.GetWorldAnchorPosition())
                {
                    // Reset the highlight
                    tileHighlight.HighlightPositions = null;
                }
                else
                {
                    SetupEntityMovement(selectedEntityId.Value, tileHighlight);
                }
            }

            // Tile is clicked to select the entity on the tile
            else if (entityOnTileId.HasValue && !selectedEntityId.HasValue)
            {
                SelectEntity(entityOnTileId.Value, selectedTile);
            }
        }

        public Entity GetSelectedEntity()
        {
            foreach (int entityId in ActiveEntities)
            {
                if (_selectedMapper.Has(entityId))
                {
                    return this.GetEntity(entityId);
                }
            }

            return null;
        }

        private void SetupEntityMovement(int entityId, TileHighlightComponent tileHighlightComponent)
        {
            Queue<Vector2> path = new Queue<Vector2>();

            tileHighlightComponent.HighlightPositions.ForEach(tp => path.Enqueue(tp));

            _movementMapper.Put(entityId, new MovementComponent
            {
                Destination = tileHighlightComponent.PossiblePositions.Last(),
                PathToDestination = path
            });
        }

        private void GetPathOfMovement(int entityId, TileHighlightComponent tileHighlightComponent, BoardTile selectedTile)
        {
            PositionComponent pos = _positionMapper.Get(entityId);
            BoardTile onTile = _boardGrid[Coordinates.WorldToBoard(pos.TilePosition)];
            Vector2 destTileAnchor = selectedTile.Position.GetWorldAnchorPosition();
            bool isValid = tileHighlightComponent.PossiblePositions.Contains(destTileAnchor);

            // This is likely a deslection if the same tile
            if (!onTile.IsEqual(selectedTile) && isValid)
            {
                // We need to determine the path that entity will take
                List<BoardTile> path = _boardGrid.ShortestPath(onTile, selectedTile);
                // Set for verification
                tileHighlightComponent.CheckPosition = destTileAnchor;
                // Add the anchor positions to the queue of the path
                tileHighlightComponent.HighlightPositions = new List<Vector2>();
                foreach (BoardTile bt in path)
                {
                    tileHighlightComponent.HighlightPositions.Add(bt.Position.GetWorldAnchorPosition());
                }
            }
        }

        private void DeselectEntity(int entityId, BoardTile selectedTile)
        {
            TurnComponent turn = _turnMapper.Get(entityId);
            PositionComponent pos = _positionMapper.Get(entityId);

            // If they are deslect them
            if (!turn.ReadyForTurn && selectedTile.Position.GetWorldAnchorPosition() == pos.TilePosition)
            {
                _selectedMapper.Delete(entityId);
                _attackingMapper.Delete(entityId);
                _tileHighlightMapper.Delete(entityId);
            }
        }

        private void SelectEntity(int entityId, BoardTile selected_tile)
        {
            // Get the componenets
            TurnComponent turn = _turnMapper.Get(entityId);
            StatsComponent stats = _statsMapper.Get(entityId);
            // Created the selected
            SelectedComponent selected = new SelectedComponent();
            // If not ready for turn disadvantage entity
            selected.Disadvantaged = !turn.ReadyForTurn;
            // This entity was selected
            _selectedMapper.Put(entityId, selected);
            // Finally get all the possible tile anchors
            List<Vector2> tiles =
                selected_tile.BoardGrid.Flood(selected_tile, stats.GetMovementDistance(selected.Disadvantaged))
                .Select(bt => bt.Position.GetWorldAnchorPosition()).ToList();
            // Filter blocked tiles
            List<Vector2> ent_positions = new List<Vector2>();
            foreach (int entId in ActiveEntities)
            {
                PositionComponent pos = _positionMapper.Get(entId);
                if (pos != null) ent_positions.Add(pos.TilePosition);
            }
            tiles.RemoveAll(t => ent_positions.Contains(t));
            // Set movements possible movement tiles
            _tileHighlightMapper.Put(entityId, new TileHighlightComponent
            {
                TileHighlight = TileHighlight.Movement,
                PossiblePositions = tiles
            });
            // Move camera to entity
            this._battle.PanCamera(selected_tile.Position.GetWorldAnchorPosition());
        }
    }
}
