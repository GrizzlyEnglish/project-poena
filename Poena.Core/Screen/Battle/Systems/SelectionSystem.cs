using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Poena.Core.Common.Enums;
using Poena.Core.Screen.Battle.Board;
using Poena.Core.Screen.Battle.Components;

namespace Poena.Core.Screen.Battle.Systems
{
    public class SelectionSystem : EntityUpdateSystem
    {
        private readonly BoardGrid _boardGrid;
        private readonly Battle _battle;
        private ComponentMapper<SelectedComponent> _selectedMapper;
        private ComponentMapper<TurnComponent> _turnMapper;
        private ComponentMapper<PositionComponent> _positionMapper;
        private ComponentMapper<AttackingComponent> _attackingMapper;
        private ComponentMapper<StatsComponent> _statsMapper;
        private ComponentMapper<TileHighlightComponent> _tileHighlightMapper;

        public SelectionSystem(BoardGrid boardGrid, Battle battle) 
            : base(Aspect.One(typeof(TurnComponent)))
        {
            this._boardGrid = boardGrid;
            this._battle = battle;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _selectedMapper = mapperService.GetMapper<SelectedComponent>();
            _positionMapper = mapperService.GetMapper<PositionComponent>();
            _turnMapper = mapperService.GetMapper<TurnComponent>();
            _attackingMapper = mapperService.GetMapper<AttackingComponent>();
            _statsMapper = mapperService.GetMapper<StatsComponent>();
            _tileHighlightMapper = mapperService.GetMapper<TileHighlightComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            BoardTile selectedTile = _boardGrid.GetSelectedTile();
            int? selectedEntityId = null;

            foreach (int entityId in ActiveEntities)
            {
                if (_selectedMapper.Has(entityId))
                {
                    selectedEntityId = entityId;
                    break;
                }
            }

            // If we have an event lets check if we seleced an entity
            if (selectedTile != null && selectedEntityId.HasValue)
            {
                DeselectEntity(selectedEntityId.Value, selectedTile);
                return;
            }

            foreach (int entityId in ActiveEntities)
            {
                TurnComponent turn = _turnMapper.Get(entityId);
                PositionComponent pos = _positionMapper.Get(entityId);
                SelectedComponent selected = _selectedMapper.Get(entityId);
                // Check if the entities anchor point is inside the tile or they are prepared for their turn
                if ( 
                    (turn != null && turn.ReadyForTurn && selected == null) ||
                    (selectedTile != null && pos != null && selectedTile.Position.GetWorldAnchorPosition() == pos.TilePosition)
                    )
                {
                    this.SelectEntity(entityId, selectedTile ?? _boardGrid[pos.TilePosition]);
                    // Mark tile as used
                    _boardGrid.ClearSelectedTile();
                    break;
                }
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

            // Mark tile as used
            _boardGrid.ClearSelectedTile();
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
            _tileHighlightMapper.Put(entityId, new TileHighlightComponent { 
                TileHighlight = TileHighlight.Movement,
                TilePositions = tiles
            });
            // Move camera to entity
            this._battle.PanCamera(selected_tile.Position.GetWorldAnchorPosition());
        }
        
    }
}
