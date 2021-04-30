using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Poena.Core.Common;
using Poena.Core.Common.Enums;
using Poena.Core.Screen.Battle.Board;
using Poena.Core.Screen.Battle.Components;

namespace Poena.Core.Screen.Battle.Systems
{
    public class TileHighlightSystem : EntityDrawSystem
    {
        private readonly Dictionary<TileHighlight, Texture2D> _textures;
        private readonly BoardGrid _boardGrid;
        private readonly SpriteBatch _spriteBatch;
        private ComponentMapper<PositionComponent> _positionMapper;
        private ComponentMapper<SelectedComponent> _selectedMapper;
        private ComponentMapper<AttackingComponent> _attackingMapper;
        private ComponentMapper<MovementComponent> _movementMapper;

        public TileHighlightSystem(SpriteBatch batch, BoardGrid boardGrid) 
            : base(Aspect.One(typeof(SelectedComponent), typeof(MovementComponent), typeof(PositionComponent)))
        {
            _textures = new Dictionary<TileHighlight, Texture2D>();
            _spriteBatch = batch;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _attackingMapper = mapperService.GetMapper<AttackingComponent>();
            _movementMapper = mapperService.GetMapper<MovementComponent>();
            _selectedMapper = mapperService.GetMapper<SelectedComponent>();
            _positionMapper = mapperService.GetMapper<PositionComponent>();
        }

        public override void Draw(GameTime gameTime)
        {
            // Store a list of tiles that need highlights
            Dictionary<Coordinates, List<TileHighlight>> tile_coordinates = new Dictionary<Coordinates, List<TileHighlight>>();

            // Loop all the entities and highlight tile as necessary
            foreach (int entityId in ActiveEntities)
            {
                // See if we are moving so we can render it
                PositionComponent pos = _positionMapper.Get(entityId);
                MovementComponent movement = _movementMapper.Get(entityId);
                SelectedComponent selected = _selectedMapper.Get(entityId);
                AttackingComponent attacking = _attackingMapper.Get(entityId);

                List<Vector2> tile_points = new List<Vector2>();
                
                // Entity is currently moving
                if (movement != null)
                {
                    tile_points = movement.path_to_destination.ToList();
                }

                // Entity is selected and showing possible moves
                else if (selected != null)
                {
                    tile_points = selected.possible_positions;
                }

                // Loop the positions and tag tiles as movement
                foreach (Vector2 path_spot in tile_points)
                {
                    Point p = Coordinates.WorldToBoard(path_spot);
                    Coordinates coordinates = Coordinates.BoardToWorld(p);
                    if (!tile_coordinates.ContainsKey(coordinates))
                    {
                        tile_coordinates.Add(coordinates, new List<TileHighlight>());
                    }
                    tile_coordinates[coordinates].Add(TileHighlight.Movement);
                    if (attacking != null)
                    {
                        tile_coordinates[coordinates].Add(TileHighlight.Attack);
                    }
                }
            }

            foreach (Coordinates coordinates in tile_coordinates.Keys)
            {
                TileHighlight highlight = tile_coordinates[coordinates].OrderByDescending(t => t).First();
                _spriteBatch.Draw(_textures[highlight], coordinates.AsVector2(), Color.White);
            }

            // TODO: Handle hovering tile
            BoardTile hoveringTile = null;
            if (hoveringTile != null) {
                Coordinates coordinates = hoveringTile.RenderCoordinates();
                _spriteBatch.Draw(_textures[TileHighlight.Movement], coordinates.AsVector2(), Color.White);
            }
        }
    }
}
