using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Poena.Core.Screen.Battle.Board;
using Poena.Core.Screen.Battle.Components;

namespace Poena.Core.Screen.Battle.Systems
{
    public class SelectionSystem : EntityUpdateSystem
    {
        private readonly BoardGrid _boardGrid;
        private readonly OrthographicCamera _camera;
        private ComponentMapper<SelectedComponent> _selectedMapper;
        private ComponentMapper<TurnComponent> _turnMapper;
        private ComponentMapper<PositionComponent> _positionMapper;
        private ComponentMapper<AttackingComponent> _attackingMapper;
        private ComponentMapper<StatsComponent> _statsMapper;

        public SelectionSystem(BoardGrid boardGrid, OrthographicCamera cam) 
            : base(Aspect.One(typeof(TurnComponent)))
        {
            this._boardGrid = boardGrid;
            this._camera = cam;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _selectedMapper = mapperService.GetMapper<SelectedComponent>();
            _positionMapper = mapperService.GetMapper<PositionComponent>();
            _turnMapper = mapperService.GetMapper<TurnComponent>();
            _attackingMapper = mapperService.GetMapper<AttackingComponent>();
            _statsMapper = mapperService.GetMapper<StatsComponent>();
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
            if (selectedTile != null)
            {
                // See if somone is already selected
                if (selectedEntityId.HasValue)
                {
                    TurnComponent turn = _turnMapper.Get(selectedEntityId.Value);
                    PositionComponent pos = _positionMapper.Get(selectedEntityId.Value);

                    // If they are deslect them
                    if (!turn.ready_for_turn && selectedTile.Position.GetWorldAnchorPosition() == pos.TilePosition)
                    {
                        _selectedMapper.Delete(selectedEntityId.Value);
                        _attackingMapper.Delete(selectedEntityId.Value);
                    }

                    // Mark tile as used
                    _boardGrid.ClearSelectedTile();

                    return;
                }    
            }

            foreach (int entityId in ActiveEntities)
            {
                TurnComponent turn = _turnMapper.Get(entityId);
                PositionComponent pos = _positionMapper.Get(entityId);
                SelectedComponent selected = _selectedMapper.Get(entityId);
                // Check if the entities anchor point is inside the tile or they are prepared for their turn
                if ( 
                    (turn != null && turn.ready_for_turn && selected == null) ||
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

        private void SelectEntity(int entityId, BoardTile selected_tile)
        {
            // Get the componenets
            TurnComponent turn = _turnMapper.Get(entityId);
            StatsComponent stats = _statsMapper.Get(entityId);
            // Created the selected
            SelectedComponent selected = new SelectedComponent();
            // If not ready for turn disadvantage entity
            selected.disadvantaged = !turn.ready_for_turn;
            // This entity was selected
            _selectedMapper.Put(entityId, selected);
            // Finally get all the possible tile anchors
            List<Vector2> tiles =
                selected_tile.BoardGrid.Flood(selected_tile, stats.GetMovementDistance(selected.disadvantaged))
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
            selected.possible_positions = tiles;
            // Move camera to entity
            this._camera.LookAt(selected_tile.Position.GetWorldAnchorPosition());
        }
        
    }
}
