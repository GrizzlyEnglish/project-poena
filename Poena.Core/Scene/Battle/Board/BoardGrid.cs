using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Poena.Core.Common;
using Poena.Core.Common.Interfaces;
using Poena.Core.Extensions;

namespace Poena.Core.Scene.Battle.Board
{

    /*
     * BoardGrid is a wrapper class for the array of tiles
     * Used for ease of access to render and update tiles
     * 
     */

    public class BoardGrid
    {
        public RectangleF GridBounds { get; private set; }

        private BoardTile[,,] Grid;
        private BoardTile ClickedTile;

        public int Width { get { return Grid.GetLength(1);  } }
        public int Length { get { return Grid.GetLength(0); } }

        public BoardTile HoveringTile { get; private set; }

        public BoardGrid(BoardTile[,,] grid_tiles)
        {
            this.Grid = grid_tiles;
            this.ForEach(bt => bt.InjectBoard(this));
            this.SetBounds();
        }

        public BoardTile this[Point coordinates]
        {
            get
            {
                int l = coordinates.X;
                int h = coordinates.Y;

                return this[l, h, 0];
            }
        }

        public BoardTile this[int l, int w, int h]
        {
            get
            {
                if (this.WithinBounds(l, w, h))
                {
                    return this.Grid[l, w, h];
                } else
                {
                    return null;
                }
            }
            set
            {
                this.Grid[l, w, h] = value;
            }
        }

        public BoardTile this[BoardGridPosition bgp]
        {
            get
            {
                return this[bgp.GridSlot.x, bgp.GridSlot.y, bgp.GridSlot.z];
            }
        }

        private void ForEach(Action<BoardTile> action)
        {
            for (int z = 0; z < 2; z++)
            {
                for (int w = 0; w < this.Width; w++)
                {
                    for (int l = 0; l < this.Length; l++)
                    {
                        BoardTile bt = this.Grid[l, w, z];
                        if (bt != null) action(bt);
                    }
                }
            }
        }

        public void Initialize()
        {
            this.ForEach(bt => {
                bt.Initialize();
            });
        }

        public bool HandleMouseClicked(MouseEvent mouseEvent)
        {
            // First lets do some math do narrow the grid way down
            Point p = Coordinates.WorldToBoard(mouseEvent.UnprojectedPosition);

            // TODO: Lets looks and see if there is a tile in the same slot but on the higher realm

            ClickedTile = this[p.X, p.Y, 0];

            return ClickedTile != null;
        }

        public void HandleMouseMoved(MouseEvent mouseEvent)
        {
            Point p = Coordinates.WorldToBoard(mouseEvent.UnprojectedPosition);

            HoveringTile = this[p.X, p.Y, 0];
        }

        public BoardTile GetClickedTile()
        {
            return ClickedTile;
        }

        public void ClearClickedTile()
        {
            ClickedTile = null;
        }
        
        public void LoadContent(ContentManager contentManager)
        {
            this.ForEach(bt => {
                if (bt != null)
                {
                    bt.LoadContent(contentManager);
                }
            });
        }

        public void Render(SpriteBatch spriteBatch, RectangleF cameraBounds)
        {
            this.ForEach(bt => {
                if (bt != null)
                {
                    bt.Render(spriteBatch, cameraBounds);
                }
            });

            if (Config.DEBUG_RENDER)
            {
                spriteBatch.DrawRectangle(GridBounds, Color.Red, 30);
            }
        }
        
        public List<BoardTile> CircleAroundTile(BoardTile bgt, int radius, bool includeCenter = false,
            bool includePrevious = false, bool addVariation = false, bool includeEdges = true)
        {
            List<BoardTile> bgts = new List<BoardTile>();
            bgt.Position.CircleAroundTile(radius,
                includeCenter, includePrevious, addVariation, includeEdges).ForEach(bgp => {
                    BoardTile b = this[bgp];
                    if (b != null) bgts.Add(b);
                });
            return bgts;
        }

        public bool WithinBounds(int l, int w, int z)
        {
            return l >= 0 && l < this.Length && w >= 0 && w < this.Width && z >= 0 && z < 2;
        }

        private void SetBounds()
        {
            //Scale back the top and the bottom
            int width_scale = (int)(this.Width * .25);
            int length_scale = (int)(this.Length * .25);

            //Get the points
            Coordinates left = Coordinates.BoardToWorld(0, this.Length);
            Coordinates top = Coordinates.BoardToWorld(-width_scale, -length_scale);
            Coordinates right = Coordinates.BoardToWorld(this.Width, 0);
            Coordinates bottom = Coordinates.BoardToWorld(this.Width + width_scale, this.Length + length_scale);

            //Find the width and the height
            int bounds_width = right.x - left.x;
            int bounds_height = bottom.y - top.y;

            int x = left.x;
            int y = top.y - (bounds_height / 2); //Shift back y half the height to center

            bounds_height += bounds_height; //Double the height to center

            this.GridBounds = new RectangleF(x, y, bounds_width, bounds_height);
        }

        public List<BoardTile> ShortestPath(BoardGridPosition start, BoardGridPosition end)
        {
            BoardTile bt_start = this[start];
            BoardTile bt_end = this[end];

            return ShortestPath(bt_start, bt_end);
        }

        public List<BoardTile> ShortestPath(BoardTile start, BoardTile end, bool addNoise = false, int factor = 0, int maxOffset = 5)
        {
            List<BoardTile> path = Path(start, end);
            if (addNoise) path = AddNoise(path, factor, maxOffset);
            return path;
        }

        private List<BoardTile> Path(BoardTile start, BoardTile end)
        {
            List<PathNode> open = new List<PathNode>();
            List<PathNode> closed = new List<PathNode>();

            PathNode node = new PathNode(start);

            bool searching = true;
            DateTime start_time = DateTime.Now;

            while (searching)
            {
                if ((DateTime.Now - start_time).Seconds >= 5)
                {
                    return node.ToList();
                }

                if (node.slot.IsEqual(end))
                {
                    return node.ToList();
                }

                //Add current node to the closed list
                closed.Add(node);

                //Get a circle around node
                List<BoardGridPosition> neighbors =
                    node.slot.Position.CircleAroundTile(1, includeEdges: false);

                //Loop each neighbor to determine what to do
                foreach (BoardGridPosition pos in neighbors)
                {
                    BoardTile neighbor = this[pos];

                    if (neighbor != null)
                    {
                        PathNode new_node = new PathNode(neighbor, node);

                        new_node.SetHeuristics(end);

                        //Only look if the board grid is there and not already closed
                        if (new_node.f > 0 && closed.Find(n => n.slot.IsEqual(new_node.slot)) == null)
                        {
                            if (open.Find(n => n.slot.IsEqual(new_node.slot)) == null)
                            {
                                open.Add(new_node);
                            }
                            else
                            {
                                PathNode prev_node = open.Find(n => n.slot.IsEqual(new_node.slot));
                                if (prev_node.f > new_node.f)
                                {
                                    prev_node.cost = new_node.cost;
                                }
                            }
                        }
                    }
                }

                if (open.Count == 0) break;

                //Grab the lowest f score to start the next loop
                float min_f = open.Min(n => n.f);
                node = open.FirstOrDefault(n => n.f == min_f);
                open = open.Where(n => !n.Equals(node)).ToList();
            }

            return null;
        }

        public BoardTile NextSlot(BoardTile bgt, int scale = 1)
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            int d = rand.Next(Enum.GetNames(typeof(TileDirections)).Length);

            for (int i = 0; i < scale; i++)
            {
                BoardGridPosition pos = bgt.Position.NextPosition((TileDirections)d, scale);
                bgt = this[pos];

                if (bgt == null) break;
            }

            return bgt;
        }


        public List<BoardTile> AddNoise(List<BoardTile> path, int factor, int maxOffset = 1)
        {
            //Based on the factor grab every x amount of tiles
            Random r = new Random();
            List<BoardTile> points = new List<BoardTile>() { path.ElementAt(0) };
            List<BoardTile> new_path = new List<BoardTile>();

            int i = factor;

            while (i < path.Count)
            {
                BoardTile bgt = path.ElementAt(i);

                int scale = r.Next(0, maxOffset);

                do
                {
                    bgt = this.NextSlot(bgt, scale);
                    scale--;
                } while (bgt == null && scale > 0);

                if (bgt == null)
                {
                    //Just use the original point
                    bgt = path.ElementAt(i);
                }

                BoardGridPosition pos = bgt.Position;

                points.Add(this[pos]);

                i += factor;
            }

            points.Add(path.ElementAt(path.Count - 1));

            //Rebuild the path offsetting the tile before setting
            for (int i2 = 1; i2 < points.Count; i2++)
            {
                List<BoardTile> segment = ShortestPath(points.ElementAt(i2 - 1), points.ElementAt(i2));
                new_path.AddRange(segment);
            }

            return new_path.Distinct().ToList();
        }

        public List<BoardTile> Flood(BoardTile source, int distance)
        {
            List<BoardTile> tiles = new List<BoardTile>();
            DateTime start_time = new DateTime();
            this.ForEach(bt => {
                if (!tiles.Contains(bt))
                {
                    List<BoardTile> path = this.ShortestPath(source, bt);
                    if (path.Count <= distance)
                    {
                        path.ForEach(p =>
                        {
                            if (!tiles.Contains(p)) tiles.Add(p);
                        });
                    }
                }
            });
            return tiles;
        }

        private class PathNode
        {
            public PathNode(BoardTile bgt, PathNode parent = null)
            {
                this.slot = bgt;
                this.parent = parent;
                this.SetCost(parent?.slot);
            }

            //Path
            public PathNode parent { get; set; }

            //Current slot
            public BoardTile slot { get; set; }

            //Combination
            public float f { get { return heuristic + cost; } }

            //Movement cost from point to start
            public float cost { get; set; }

            //A straight line cost
            public float heuristic { get; set; }

            public void SetHeuristics(BoardTile end)
            {
                Coordinates end_pos = end.Position.GridSlot;
                Coordinates this_pos = this.slot.Position.GridSlot;

                this.heuristic = (Math.Abs(this_pos.x - end_pos.x) +
                    Math.Abs(this_pos.y - end_pos.y) + Math.Abs(this_pos.z - end_pos.z)) / 2;
            }

            public void SetCost(BoardTile from, IRouteable mover = null)
            {
                this.cost = this.slot.GetMovementCost(mover);
            }

            public bool IsEqual(BoardTile bgt)
            {
                return bgt.Equals(slot);
            }

            public List<BoardTile> ToList()
            {
                List<BoardTile> path = new List<BoardTile>();
                PathNode node = this;
                while (node != null)
                {
                    path.Add(node.slot);
                    node = node.parent;
                }
                path.Reverse();
                return path;
            }

        }



    }
}
