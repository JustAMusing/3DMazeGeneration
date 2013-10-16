using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _3DMazeGeneration
{
    class MazeRunner
    {
        int x, y, z;
        int xMod = 10;
        int yMod = 4;
        int[] playerCoor, endCoor;
        char[][][] maze;
        bool u, d, l, r, f, b;

        public MazeRunner(Maze _m)
        {
            maze = _m.GetLayout();
            x = _m.x;
            y = _m.y;
            z = _m.z;
            playerCoor = _m.startCoor;
            endCoor = _m.endCoor;
        }

        public void Run()
        {
            Console.Clear();
            while (!HasWon())
            {
                Play();
            }
            Win();
        }

        void Play()
        {
            GetMoves();
            WriteMaze();
            Move();
            Console.Read();
        }

        void Win()
        {
            WriteMaze();
            Console.WriteLine("Player has completed maze!");
            Console.WriteLine("Press [enter] to continue.");
            Console.ReadLine();
            Console.Clear();
        }

        void GetMoves()
        {
            //Wall to left of player: (z, ((4*y)+2), (4*x))
            //Wall to right of player: (z, ((4*y)+2), (4*(x+1)))
            //Wall in front of player: (z, (4*(y+1)), ((4*x)+2))
            //Wall in back of player: (z, (4*y), ((4*x)+2))
            //Lower: (z, ((4*y)+2), ((4*x)+1))
            //Upper: (z, ((4*y)+2), ((4*x)+3))

            l = (maze[playerCoor[0]][((4 * playerCoor[1]) + 2)][(4 * playerCoor[2])] == ' ') && (playerCoor[2] != (0));
            r = (maze[playerCoor[0]][((4 * playerCoor[1]) + 2)][(4 * (playerCoor[2] + 1))] == ' ') && (playerCoor[2] != (x - 1));
            f = (maze[playerCoor[0]][(4 * (playerCoor[1] + 1))][((4 * playerCoor[2]) + 2)] == ' ') && (playerCoor[1] != (y - 1));
            b = (maze[playerCoor[0]][(4 * playerCoor[1])][((4 * playerCoor[2]) + 2)] == ' ') && (playerCoor[1] != 0);
            d = (maze[playerCoor[0]][((4 * playerCoor[1]) + 2)][((4 * playerCoor[2]) + 1)] == '-');
            u = (maze[playerCoor[0]][((4 * playerCoor[1]) + 2)][((4 * playerCoor[2]) + 3)] == '+');
        }

        void Move()
        {
            char input = 'z';
            List<char> possMoves = new List<char>();

            Console.WriteLine("Player can...");
            if (f)
            {
                Console.WriteLine(" Move forward (f)");
                possMoves.Add('f');
            }
            if (b)
            {
                Console.WriteLine(" Move back (b)");
                possMoves.Add('b');
            }
            if (l)
            {
                Console.WriteLine(" Move left (l)");
                possMoves.Add('l');
            }
            if (r)
            {
                Console.WriteLine(" Move right (r)");
                possMoves.Add('r');
            }
            if (u)
            {
                Console.WriteLine(" Ascend (a)");
                possMoves.Add('a');
            }
            if (d)
            {
                Console.WriteLine(" Descend (d)");
                possMoves.Add('d');
            }

            while (!possMoves.Contains(input))
            {
                input = Console.ReadKey().KeyChar;
            }

            if (input == 'f')
            {
                playerCoor[1]++;
            }
            if (input == 'b')
            {
                playerCoor[1]--;
            }
            if (input == 'l')
            {
                playerCoor[2]--;
            }
            if (input == 'r')
            {
                playerCoor[2]++;
            }
            if (input == 'a')
            {
                playerCoor[0]++;
            }
            if (input == 'd')
            {
                playerCoor[0]--;
            }
        }

        bool HasWon()
        {
            return ((playerCoor[0] == endCoor[0]) && (playerCoor[1] == endCoor[1]) && (playerCoor[2] == endCoor[2]));
        }

        void WriteMaze()
        {
            int y0, x0;
            y0 = ((playerCoor[1] / yMod) * yMod);
            x0 = ((playerCoor[2] / xMod) * xMod);

            for (int _y = (4 * y0); (_y < ((4 * y) + 1)) && (_y < ((4 * (y0 + yMod)) + 1)); _y++)
            {
                for (int _x = (4 * x0); (_x < ((4 * x) + 1)) && (_x < ((4 * (x0 + xMod)) + 1)); _x++)
                {
                    if ((_y == ((4 * playerCoor[1]) + 2)) && (_x == ((4 * playerCoor[2]) + 2)))
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
