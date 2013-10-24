using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _3DMazeGeneration
{
    class MazeRunner
    {
        readonly int x, y, z;
        int xMod = 20;
        int yMod = 20;
        internal readonly int cellWidth;
        internal readonly int cellHeight;
        internal readonly int wallWidth;
        internal readonly int wallHeight;
        int[] playerCoor;
        int [][] endCoor;
        char[][][] maze;
        bool u, d, l, r, f, b;

        public MazeRunner(Maze _m)
        {
            maze = _m.GetLayout();
            x = _m.x;
            y = _m.y;
            z = _m.z;
            cellWidth = _m.cellWidth;
            cellHeight = _m.cellHeight;
            wallWidth = _m.wallWidth;
            wallHeight = _m.wallHeight;
            playerCoor = _m.playerCoor;
            endCoor = _m.endCoorRange;
        }

        public void Run()
        {
            Draw();
            while (!HasWon())
            {
                if (Console.KeyAvailable)
                {
                    Move();
                    Draw();
                }
            }
            Win();
        }

        void Draw()
        {
            Console.Clear();
            WriteMaze();
            GetMoves();
        }

        void Win()
        {
            Draw();
            Console.WriteLine("Player has completed maze!");
            Console.WriteLine("Press [enter] to continue.");
            Console.ReadLine();
            Console.Clear();
        }

        void GetMoves()
        {
            //Wall to left of player: (z, (((cellHeight+wallHeight)*y)+((cellHeight+wallHeight)/2)), ((cellWidth+wallWidth)*x))
            //Wall to right of player: (z, (((cellHeight+wallHeight)*y)+((cellHeight+wallHeight)/2)), ((cellWidth+wallWidth)*(x+1)))
            //Wall in front of player: (z, ((cellHeight+wallHeight)*(y+1)), (((cellWidth+wallWidth)*x)+wallWidth))
            //Wall in back of player: (z, ((cellHeight+wallHeight)*y), (((cellWidth+wallWidth)*x)+wallWidth))
            //Lower: (z, (((cellHeight+wallHeight)*y)+((cellHeight+wallHeight)/2)), (((cellWidth+wallWidth)*x)+wallWidth-1))
            //Upper: (z, (((cellHeight+wallHeight)*y)+((cellHeight+wallHeight)/2)), (((cellWidth+wallWidth)*(x+1))+wallWidth+1))

            //Get left
            if ((playerCoor[0] >= 0) && (playerCoor[0] < maze.Length) && (playerCoor[1] >= 0) && (playerCoor[1] < maze[0].Length) && ((playerCoor[2] - 1) >= 0) && ((playerCoor[2] - 1) < maze[0][0].Length))
            {
                l = (maze[playerCoor[0]][playerCoor[1]][playerCoor[2] - 1] != '#');
            }
            else
            {
                l = false;
            }
            //Get right
            if ((playerCoor[0] >= 0) && (playerCoor[0] < maze.Length) && (playerCoor[1] >= 0) && (playerCoor[1] < maze[0].Length) && ((playerCoor[2] + 1) >= 0) && ((playerCoor[2] + 1) < maze[0][0].Length))
            {
                r = (maze[playerCoor[0]][playerCoor[1]][playerCoor[2] + 1] != '#');
            }
            else
            {
                r = false;
            }
            //Get back
            if ((playerCoor[0] >= 0) && (playerCoor[0] < maze.Length) && ((playerCoor[1] - 1) >= 0) && ((playerCoor[1] - 1) < maze[0].Length) && (playerCoor[2] >= 0) && (playerCoor[2] < maze[0][0].Length))
            {
                b = (maze[playerCoor[0]][playerCoor[1] - 1][playerCoor[2]] != '#');
            }
            else
            {
                b = false;
            }
            //Get forward
            if ((playerCoor[0] >= 0) && (playerCoor[0] < maze.Length) && ((playerCoor[1] + 1) >= 0) && ((playerCoor[1] + 1) < maze[0].Length) && (playerCoor[2] >= 0) && (playerCoor[2] < maze[0][0].Length))
            {
                f = (maze[playerCoor[0]][playerCoor[1] + 1][playerCoor[2]] != '#');
            }
            else
            {
                f = false;
            }
            //Get up and down
            if ((playerCoor[0] >= 0) && (playerCoor[0] < maze.Length) && (playerCoor[1] >= 0) && (playerCoor[1] < maze[0].Length) && (playerCoor[2] >= 0) && (playerCoor[2] < maze[0][0].Length))
            {
                u = (maze[playerCoor[0]][playerCoor[1]][playerCoor[2]] == '+');
                d = (maze[playerCoor[0]][playerCoor[1]][playerCoor[2]] == '-');
            }
            else
            {
                u = false;
                d = false;
            }

            //Write controls
            Console.WriteLine("Controls:");
            Console.WriteLine();
            Console.WriteLine("   [W]");
            Console.WriteLine("[A][S][D]");
            Console.WriteLine();
            if (d)
            {
                Console.WriteLine("[SPACE] to Descend");
            }
            if (u)
            {
                Console.WriteLine("[SPACE] to Ascend");
            }
        }

        void Move()
        {
            ConsoleKey move = Console.ReadKey(false).Key;

            if ((move == ConsoleKey.W) && b)
            {
                playerCoor[1]--;
                Console.Clear();
            }
            else if ((move == ConsoleKey.S) && f)
            {
                playerCoor[1]++;
                Console.Clear();
            }
            else if ((move == ConsoleKey.A) && l)
            {
                playerCoor[2]--;
                Console.Clear();
            }
            else if ((move == ConsoleKey.D) && r)
            {
                playerCoor[2]++;
                Console.Clear();
            }
            else if (move == ConsoleKey.Spacebar)
            {
                if (d)
                {
                    playerCoor[0]--;
                    Console.Clear();
                }
                if (u)
                {
                    playerCoor[0]++;
                    Console.Clear();
                }

            }
        }

        bool HasWon()
        {
            return ((playerCoor[0] == endCoor[0][0]) && 
                    (playerCoor[1] >= endCoor[0][1]) && 
                    (playerCoor[1] <= endCoor[1][1]) && 
                    (playerCoor[2] >= endCoor[0][2]) && 
                    (playerCoor[2] <= endCoor[1][2]));
        }

        void WriteMaze()
        {
            int y0, x0;
            y0 = (playerCoor[1] - yMod);
            x0 = (playerCoor[2] - xMod);
            if (y0 < 0)
            {
                y0 = 0;
            }
            if (x0 < 0)
            {
                x0 = 0;
            }

            for (int _y = y0; (_y < maze[0].Length) && (_y <= (y0 + (2 * yMod))); _y++)
            {
                for (int _x = x0; (_x < maze[0][0].Length) && (_x < (x0 + (2 * xMod))); _x++)
                {
                    if ((_y == playerCoor[1]) && (_x == playerCoor[2]))
                    {
                        Console.Write('X');
                    }
                    else
                    {
                        Console.Write(maze[playerCoor[0]][_y][_x]);
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
