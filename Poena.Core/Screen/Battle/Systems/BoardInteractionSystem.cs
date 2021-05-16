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
        private ComponentMapper<DamageComponent> _damageMapper;
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
            _damageMapper = mapperService.GetMapper<DamageComponent>();
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

            if (selectedEntityId.HasValue)
            {
                TileHighlightComponent tileHighlightComponent = _tileHighlightMapper.Get(selectedEntityId.Value);
                bool isMovement = tileHighlightComponent.TileHighlight == TileHighlight.Movement;
                bool isAttacking = tileHighlightComponent.TileHighlight == TileHighlight.Attack;
                bool highlightingMovement = isMovement && tileHighlightComponent.HighlightCheck;
                bool selectedTileInPossible = tileHighlightComponent.PossiblePositions.Contains(selectedTile.Position.GetWorldAnchorPosition());
                bool selectedTileIsDestination = tileHighlightComponent.CheckPosition == selectedTile.Position.GetWorldAnchorPosition();

                // Go by the most specific to least specific move
                if (isAttacking)
                {
                    // Nest if is by design, we don't want to do others if attacking
                    if (entityOnTileId.HasValue)
                    {
                        // Attack the entity
                        if (!tileHighlightComponent.HighlightCheck)
                        {
                            // Set the entity tile as being highlighted for attack
                            tileHighlightComponent.HighlightPositions = new List<Vector2>() { selectedTile.Position.GetWorldAnchorPosition() };
                            // TODO: Add information on attack
                        } 
                        else
                        {
                            // Attack the entity
                            _tileHighlightMapper.Delete(selectedEntityId.Value);
                            _damageMapper.Put(entityOnTileId.Value, new DamageComponent
                            {
                                Damage = 10,
                                CurrentTime = 0
                            });
                        }
                    }
                }
                else if (isMovement && (highlightingMovement || selectedTileInPossible))
                {
                    // Highlight movement, or setup for movement
                    if (!tileHighlightComponent.HighlightCheck)
                    {
                        GetPathOfMovement(selectedEntityId.Value, tileHighlightComponent, selectedTile);
                    }
                    else if (!selectedTileIsDestination)
                    {
                        // Reset the highlight
                        tileHighlightComponent.HighlightPositions = null;
                    }
                    else
                    {
                        SetupEntityMovement(selectedEntityId.Value, tileHighlightComponent);
                    }
                }
                else
                {
                    DeselectEntity(selectedEntityId.Value);
                }
            }
            else
            {
                // Tile is clicked to select the entity on the tile
                if (entityOnTileId.HasValue && !selectedEntityId.HasValue)
                {
                    SelectEntity(entityOnTileId.Value, selectedTile);
                }
            }
        }

        public int? GetSelectedEntityId()
        {
            foreach (int entityId in ActiveEntities)
            {
                if (_selectedMapper.Has(entityId))
                {
                    return entityId;
                }
            }

            return null;
        }

        public Entity GetSelectedEntity()
        {
            int? entityId = this.GetSelectedEntityId();
            return entityId.HasValue ? this.GetEntity(entityId.Value) : null;
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

        public void SelectedEntityEndTurn()
        {
            int selectedEntityId = this.GetSelectedEntityId().Value;
            TurnComponent turn = _turnMapper.Get(selectedEntityId);
            turn.CurrentTime = 0;
            this.DeselectEntity(selectedEntityId);
        }

        public void DeselectEntity(int entityId)
        {
            TurnComponent turn = _turnMapper.Get(entityId);

            // If they are deslect them
            if (!turn.ReadyForTurn)
            {
                _selectedMapper.Delete(entityId);
                _tileHighlightMapper.Delete(entityId);
            }
        }

        public void HandleHotBarInteraction(AttackType selectedAttack)
        {
            int selectedEntityId = this.GetSelectedEntityId().Value;

            // Check if we need to selet or deselect
            if (_tileHighlightMapper.Get(selectedEntityId).TileHighlight == TileHighlight.Movement)
            {
                // Select
                SelectEntityAttack(selectedEntityId, selectedAttack);
            }
            else
            {
                DeselectEntityAttack(selectedEntityId);
            }
        }

        public void SelectEntityAttack(int entityId, AttackType selectedAttack)
        {
            Entity entity = this.GetEntity(entityId);
            PositionComponent pos = entity.Get<PositionComponent>();
            BoardTile onTile = _boardGrid[Coordinates.WorldToBoard(pos.TilePosition)];
            switch (selectedAttack)
            {
                case AttackType.Skill:
                    SkillComponent skillComponent = entity.Get<SkillComponent>();
                    List<Vector2> tiles =
                        onTile.BoardGrid.GetTilePattern(onTile, skillComponent.AttackPattern, skillComponent.Distance)
                        .Select(bt => bt.Position.GetWorldAnchorPosition()).ToList();
                    tiles.Remove(onTile.Position.GetWorldAnchorPosition());
                    entity.Attach(new TileHighlightComponent
                    {
                        PossiblePositions = tiles,
                        TileHighlight = TileHighlight.Attack,
                        AttackType = selectedAttack
                    });
                    break;
            }
        }

        public void DeselectEntityAttack(int entityId)
        {
            _tileHighlightMapper.Delete(entityId);
            SelectEntityMovement(entityId);
        }

        public void SelectEntity(int entityId, BoardTile selected_tile)
        {
            // Get the componenets
            TurnComponent turn = _turnMapper.Get(entityId);
            // Created the selected
            SelectedComponent selected = new SelectedComponent();
            // If not ready for turn disadvantage entity
            selected.Disadvantaged = !turn.ReadyForTurn;
            // This entity was selected
            _selectedMapper.Put(entityId, selected);
            SelectEntityMovement(entityId);
            // Move camera to entity
            this._battle.PanCamera(selected_tile.Position.GetWorldAnchorPosition());
        }

        public void SelectEntityMovement(int entityId)
        {
            StatsComponent stats = _statsMapper.Get(entityId);
            PositionComponent pos = _positionMapper.Get(entityId);
            BoardTile onTile = _boardGrid[Coordinates.WorldToBoard(pos.TilePosition)];
            // Finally get all the possible tile anchors
            List<Vector2> tiles =
                onTile.BoardGrid.Flood(onTile, stats.GetMovementDistance())
                .Select(bt => bt.Position.GetWorldAnchorPosition()).ToList();
            // Filter blocked tiles
            List<Vector2> ent_positions = new List<Vector2>();
            foreach (int entId in ActiveEntities)
            {
                PositionComponent entPos = _positionMapper.Get(entId);
                if (entPos != null) ent_positions.Add(entPos.TilePosition);
            }
            tiles.RemoveAll(t => ent_positions.Contains(t));
            // Set movements possible movement tiles
            _tileHighlightMapper.Put(entityId, new TileHighlightComponent
            {
                TileHighlight = TileHighlight.Movement,
                PossiblePositions = tiles
            });
        }
    }
}
