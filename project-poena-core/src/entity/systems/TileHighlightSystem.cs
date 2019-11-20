using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Project_Poena.Entity.Components;
using Project_Poena.Common.Rectangle;
using Project_Poena.Common.Coordinates;
using Project_Poena.Common.Variables;
using Project_Poena.Entity.Entities;
using Project_Poena.Entity.Managers;

namespace Project_Poena.Entity.Systems
{
    public class TileHighlightSystem : ECSystem
    {
        private Texture2D movement_marker_sprite;

        public TileHighlightSystem(SystemManager systemManager) : base(systemManager)
        {
        }

        public override void Initiliaze()
        {
            
        }
        
        public override void LoadContent(ContentManager contentManager)
        {
            //TODO: rce - Load all movement highlight variations
            this.movement_marker_sprite = contentManager.Load<Texture2D>(Variables.AssetPaths.TILE_PATH + "HEX_Highlight_Test");
        }

        public override void Render(SpriteBatch batch, RectangleF camera_bounds)
        {
            //Get all the entities that are affecting the tiles
            List<ECEntity> entities =
                this.manager.entity_manager.GetEntities(new Type[] { typeof(PositionComponent), typeof(MovementComponent), typeof(SelectedComponent) });

            //Store a list of tiles that need highlights
            Dictionary<Coordinates, List<string>> tile_coordinates = new Dictionary<Coordinates, List<string>>();

            //Loop all the entities and highlight tile as necessary
            foreach (ECEntity ent in entities)
            {
                //See if we are moving so we can render it
                PositionComponent pos = ent.GetComponent<PositionComponent>();
                MovementComponent movement = ent.GetComponent<MovementComponent>();
                SelectedComponent selected = ent.GetComponent<SelectedComponent>();

                List<Vector2> tile_points = new List<Vector2>();
                
                //Entity is currently moving
                if (movement != null)
                {
                    tile_points = movement.path_to_destination.ToList();
                }
                //Entity is selected and showing possible moves
                else if (selected != null)
                {
                    tile_points = selected.possible_positions;
                }

                //Loop the positions and tag tiles as movement
                foreach (Vector2 path_spot in tile_points)
                {
                    Point p = Coordinates.WorldToBoard(path_spot);
                    Coordinates coordinates = Coordinates.BoardToWorld(p.X, p.Y);
                    if (!tile_coordinates.ContainsKey(coordinates))
                    {
                        tile_coordinates.Add(coordinates, new List<string>());
                        tile_coordinates[coordinates] = new List<string>();
                    }
                    tile_coordinates[coordinates].Add("movement");
                }
            }

            foreach (Coordinates coordinates in tile_coordinates.Keys)
            {
                List<string> highlights = tile_coordinates[coordinates];

                //Determine the highesst precident and render that

                //TODO: rce - add system to determine what to render
                batch.Draw(this.movement_marker_sprite, coordinates.AsVector2(), Color.White);
            }
        }
    }
}
