using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Poena.Core.Common;
using Poena.Core.Common.Enums;
using Poena.Core.Entity;
using Poena.Core.Entity.Components;
using Poena.Core.Entity.Managers;
using Poena.Core.Entity.Systems;
using Poena.Core.Scene.Battle.Board;
using Poena.Core.Scene.Battle.Components;

namespace Poena.Core.Scene.Battle.Systems
{
    public class TileHighlightSystem : ECSystem
    {
        private Dictionary<TileHighlight, Texture2D> Textures;

        public TileHighlightSystem(SystemManager systemManager) : base(systemManager)
        {
            Textures = new Dictionary<TileHighlight, Texture2D>();
        }

        public override void Initiliaze()
        {
            
        }
        
        public override void LoadContent(ContentManager contentManager)
        {
            Textures.Add(TileHighlight.Movement, contentManager.Load<Texture2D>(Assets.GetTileHighlight(TileHighlight.Movement)));
            Textures.Add(TileHighlight.Attack, contentManager.Load<Texture2D>(Assets.GetTileHighlight(TileHighlight.Attack)));
        }

        public override void Update(double dt) 
        {
            // Nothing to update
        }

        public override void Render(SpriteBatch batch, RectangleF cameraBounds)
        {
            // Get all the entities that are affecting the tiles
            List<ECEntity> entities =
                this.Manager.EntityManager.GetEntities(new Type[] { 
                    typeof(PositionComponent), 
                    typeof(MovementComponent), 
                    typeof(SelectedComponent) 
                    });

            // Store a list of tiles that need highlights
            Dictionary<Coordinates, List<TileHighlight>> tile_coordinates = new Dictionary<Coordinates, List<TileHighlight>>();

            // Loop all the entities and highlight tile as necessary
            foreach (ECEntity ent in entities)
            {
                // See if we are moving so we can render it
                PositionComponent pos = ent.GetComponent<PositionComponent>();
                MovementComponent movement = ent.GetComponent<MovementComponent>();
                SelectedComponent selected = ent.GetComponent<SelectedComponent>();
                AttackingComponent attacking = ent.GetComponent<AttackingComponent>();

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
                batch.Draw(this.Textures[highlight], coordinates.AsVector2(), Color.White);
            }

            BoardTile hoveringTile = this.Manager.SceneLayer.GetLayerNode<BattleBoard>().GetHoveringTile();
            if (hoveringTile != null) {
                Coordinates coordinates = hoveringTile.RenderCoordinates();
                batch.Draw(this.Textures[TileHighlight.Movement], coordinates.AsVector2(), Color.White);
            }
        }
    }
}
