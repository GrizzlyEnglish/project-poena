using System;
using System.Collections.Generic;
using System.Linq;
using Poena.Core.Common;
using Poena.Core.Common.Interfaces;

namespace Poena.Core.Scene.Battle.Board
{
    public class BoardGridBuilder : IBuilder<BoardGrid>
    {
        private int width;
        private int height;

        private bool allow_second_layer = false;

        private Random rand;

        public BoardGridBuilder()
        {
            rand = new Random();
        }

        public BoardGridBuilder SetSize(BoardSize board_size)
        {
            //Keep it square with a little offset
            int bs = (int)board_size;
            int size = bs * 4;
            
            this.width = size + (bs * (rand.Next(0, 1) == 1 ? -1 : 1));
            this.height = size + (bs * (rand.Next(0, 1) == 1 ? -1 : 1));

            return this;
        }

        public BoardGridBuilder AllowSecondLayer()
        {
            this.allow_second_layer = true;

            return this;
        }
        
        private BoardTile[,,] GetBaseTiles()
        {
            BoardTile[,,] bt = new BoardTile[this.width, this.height, 2];
            
            List<BoardGridPosition> pos = new List<BoardGridPosition>();

            //Get the center tile
            int size = (int)this.height / 2;
            BoardGridPosition center_pos = new BoardGridPosition(size, size, 0);

            //Circle around this tile
            pos.AddRange(center_pos.CircleAroundTile(this.width/2 - 2, true, true, true, false));

            //Create the tiles
            pos.ForEach(bgp =>
            {
                Coordinates board = bgp.GridSlot;
                if (board.x > 0 && board.x < this.width && board.y > 0 && board.y < this.height)
                {
                    bt[board.x, board.y, 0] = new BoardTile(board);
                }
            });

            if (this.allow_second_layer)
            {

                //Add a few upper levels
                int count = (int)(this.width * this.height * .025);

                for (int i = 0; i < count; i++)
                {
                    Coordinates board = new Coordinates(rand.Next(0, this.width - 1), rand.Next(0, this.height - 1), 1);
                    if (board.x > 0 && board.x < this.width && board.y > 0 && board.y < this.height
                        && bt[board.x, board.y, 0] != null)
                    {
                        //This is within the board and also has a tile underneath but we need two additional checks
                        //First make sure that there is atleast 1 sircle of tiles on the ground
                        bool circleBelow = new BoardGridPosition(board.x, board.y, 0).CircleAroundTile(1).All(bgp =>
                        {
                            return board.x > 0 && board.x < this.width && board.y > 0 && board.y < this.height
                                && bt[board.x, board.y, 0] != null;
                        });

                        //Next make sure there is not another second level within 2 circles
                        bool circleAbove = new BoardGridPosition(board.x, board.y, 1).CircleAroundTile(1).All(bgp =>
                        {
                            return board.x > 0 && board.x < this.width && board.y > 0 && board.y < this.height
                                && bt[board.x, board.y, 1] == null;
                        });

                        //TODO: Fix this
                        if (circleAbove && circleBelow) bt[board.x, board.y, 1] = new BoardTile(board);
                    }
                }
            }

            return bt;
        }

        public BoardGrid Build()
        {
            //Create the array
            BoardTile[,,] grid = this.GetBaseTiles();

            //Set the tile variation

            //Set the obsticales

            //Set the events

            //Set location of enemies
            
            return new BoardGrid(grid);
        }
    }
}
