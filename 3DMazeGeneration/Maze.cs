using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _3DMazeGeneration
{
    /// <summary>
    /// Class to generate a 3D maze layout
    /// </summary>
    class Maze
    {
        internal readonly int x, y, z,
            cellWidth, cellHeight,
            wallWidth, wallHeight,
            unitWidth, unitHeight,
            layoutWidth, layoutHeight;
        internal int[] startCoor, endCoor, playerCoor;
        internal int[][] startCoorRange, endCoorRange;

        Layer layer;
        static readonly Random rand = new Random();
        char[][][] layout;

        protected static readonly Func<int, int> boundX0 = delegate(int _x) { return (3 * _x); }, boundX1 = delegate(int _x) { return ((3 * _x) + 1); }, boundX2 = delegate(int _x) { return ((3 * _x) + 2); },
            boundY0 = delegate(int _y) { return (2 * _y); }, boundY1 = delegate(int _y) { return ((2 * _y) + 1); };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_z"></param>
        public Maze(int _x, int _y, int _z)
        {
            x = _x;
            y = _y;
            z = _z;

            cellWidth = 3;
            cellHeight = 3;
            wallWidth = 1;
            wallHeight = 1;

            unitWidth = (cellWidth + wallWidth);
            unitHeight = (cellHeight + wallHeight);
            layoutWidth = ((unitWidth * x) + wallWidth);
            layoutHeight = ((unitHeight * y) + wallHeight);

            layout = new char[z][][];
            CreateMaze();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_z"></param>
        /// <param name="_cWidth"></param>
        /// <param name="_cHeight"></param>
        /// <param name="_wWidth"></param>
        /// <param name="_wHeight"></param>
        public Maze(int _x, int _y, int _z, int _cWidth, int _cHeight, int _wWidth, int _wHeight)
        {
            x = _x;
            y = _y;
            z = _z;

            cellWidth = _cWidth;
            cellHeight = _cHeight;
            wallWidth = _wWidth;
            wallHeight = _wHeight;

            unitWidth = (cellWidth + wallWidth);
            unitHeight = (cellHeight + wallHeight);
            layoutWidth = ((unitWidth * x) + wallWidth);
            layoutHeight = ((unitHeight * y) + wallHeight);

            layout = new char[z][][];
            CreateMaze();
        }

        /// <summary>
        /// 
        /// </summary>
        private void CreateMaze()
        {
            GetFirstLayer();
            for (int _z = 1; _z < (z - 1); _z++)
            {
                NextLayer();
            }
            CompleteMaze();
            CreateStartEnd();
        }

        /// <summary>
        /// 
        /// </summary>
        private void GetFirstLayer()
        {
            layer = new Layer(x, y);
            WriteLayerToCharArray();
        }

        /// <summary>
        /// 
        /// </summary>
        private void NextLayer()
        {
            layer.NextLayer();
            WriteLayerToCharArray();
        }

        /// <summary>
        /// 
        /// </summary>
        private void CompleteMaze()
        {
            layer.CompleteMaze();
            WriteLayerToCharArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public char[][][] GetLayout()
        {
            return layout;
        }

        /// <summary>
        /// 
        /// </summary>
        public void WriteLayout()
        {
            Console.WriteLine("Start: (" + startCoor[2] + ", " + startCoor[1] + ", " + startCoor[0] + ")");
            Console.WriteLine("End: (" + endCoor[2] + ", " + endCoor[1] + ", " + endCoor[0] + ")");

            for (int _z = 0; _z < z; _z++)
            {
                Console.WriteLine("Floor " + (_z + 1));

                for (int _y = 0; _y < ((4 * y) + 1); _y++)
                {
                    for (int _x = 0; _x < ((4 * x) + 1); _x++)
                    {
                            Console.Write(layout[_z][_y][_x]);
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void WriteLayerToCharArray()
        {
            bool[][] boundVals = layer.GetBoundaryValues();

            for (int _z = 0; _z < z; _z++)
            {
                //If layer empty
                if (layout[_z] == null)
                {
                    //Create x and y arrays
                    layout[_z] = new char[layoutHeight][];
                    for (int _y = 0; _y < layoutHeight; _y++)
                    {
                        layout[_z][_y] = new char[layoutWidth];
                        layout[_z][_y].DefaultIfEmpty(' ');
                    }

                    //Create outer bounds
                    for (int _x = 0; _x < layoutWidth; _x++)
                    {
                        for (int wH = 0; wH < wallHeight; wH++)
                        {
                            layout[_z][wH][_x] = '#';
                            layout[_z][LY_wallY(y - 1) + wH][_x] = '#';
                        }
                    }

                    for (int _y = 0; _y < layoutHeight; _y++)
                    {
                        for (int wW = 0; wW < wallWidth; wW++)
                        {
                            layout[_z][_y][wW] = '#';
                            layout[_z][_y][RX_wallX(x - 1) + wW] = '#';
                        }
                    }

                    //Create inner bounds
                    //For all y
                    for (int _y = 0; _y < y; _y++)
                    {
                        //For all x
                        for (int _x = 0; _x < x; _x++)
                        {
                            //Create wallX
                            if (_x != (x - 1))
                            {
                                for (int modY = 0; modY < cellHeight; modY++)
                                {
                                    for (int modX = 0; modX < wallWidth; modX++)
                                    {
                                        if (boundVals[boundY0(_y)][boundX2(_x)])
                                        {
                                            layout[_z][UY_wallX(_y) + modY][RX_wallX(_x) + modX] = '#';
                                        }
                                        else
                                        {
                                            layout[_z][UY_wallX(_y) + modY][RX_wallX(_x) + modX] = ' ';
                                        }
                                    }
                                }
                            }

                            //Create wallY
                            if (_y != (y - 1))
                            {
                                for (int modY = 0; modY < wallHeight; modY++)
                                {
                                    for (int modX = 0; modX < cellWidth; modX++)
                                    {
                                        if (boundVals[boundY1(_y)][boundX0(_x)])
                                        {
                                            layout[_z][LY_wallY(_y) + modY][LX_wallY(_x) + modX] = '#';
                                        }
                                        else
                                        {
                                            layout[_z][LY_wallY(_y) + modY][LX_wallY(_x) + modX] = ' ';
                                        }
                                    }
                                }
                            }

                            //Create corner
                            if ((_x != (x - 1)) && (_y != (y - 1)))
                            {
                                for (int modY = 0; modY < wallHeight; modY++)
                                {
                                    for (int modX = 0; modX < wallWidth; modX++)
                                    {
                                        if (boundVals[boundY1(_y)][boundX2(_x)])
                                        {
                                            layout[_z][LY_wallY(_y) + modY][RX_wallX(_x) + modX] = '#';
                                        }
                                        else
                                        {
                                            layout[_z][LY_wallY(_y) + modY][RX_wallX(_x) + modX] = ' ';
                                        }
                                    }
                                }
                            }

                            //Create upper and lower
                            int[] coor = new int[] { -1, -1 };
                            if (_z > 0)
                            {
                                for (int modY = 0; modY < cellHeight; modY++)
                                {
                                    for (int modX = 0; modX < cellWidth; modX++)
                                    {
                                        if (layout[_z - 1][UY_wallX(_y) + modY][LX_wallY(_x) + modX] == '+')
                                        {
                                            layout[_z][UY_wallX(_y) + modY][LX_wallY(_x) + modX] = '-';
                                            coor = new int[] { UY_wallX(_y) + modY, LX_wallY(_x) + modX };
                                        }
                                    }
                                }
                            }
                            if (!boundVals[boundY0(_y)][boundX1(_x)])
                            {
                                coor = Coor_upper(_y, _x, coor);
                                layout[_z][coor[0]][coor[1]] = '+';
                            }
                        }
                    }
                    //Break loop
                    break;
                }
            }

            /*
                                for (int modY = 0; modY < cellHeight; modY++)
                                {
                                    for (int modX = 0; modX < cellWidth; modX++)
                                    {
                                    }
                                }
             * */

            /*
                        char[][] layerMap = new char[layoutHeight][];
                        for (int i = 0; i < ((4 * y) + 1); i++)
                        {
                            layerMap[i] = new char[layoutWidth];
                        }

                        //Create Outer Walls
                        for (int _x = 0; _x < layoutWidth; _x++)
                        {
                            for (int wH = 0; wH < wallHeight; wH++)
                            {
                                layerMap[wH][_x] = '#';
                                layerMap[LY_wallY(y - 1) + wH][_x] = '#';
                            }
                        }
                        for (int _y = 0; _y < layoutHeight; _y++)
                        {
                            for (int wW = 0; wW < wallWidth; wW++)
                            {
                                layerMap[_y][wW] = '#';
                                layerMap[_y][LX_wallX(x - 1) + wW] = '#';
                            }
                        }

                        //Create wallXs
                        for (int _x = 0; _x < x; _x++)
                        {
                            for (int _y = 0; _y < y; _y++)
                            {
                                if (_x != (x - 1))
                                {
                                    if (boundVals[boundY0(_y)][boundX2(_x)])
                                    {
                                        layerMap[((4 * _y) + 1)][(4 * (_x + 1))] = '#';
                                        layerMap[((4 * _y) + 2)][(4 * (_x + 1))] = '#';
                                        layerMap[((4 * _y) + 3)][(4 * (_x + 1))] = '#';
                                    }
                                    else
                                    {
                                        layerMap[((4 * _y) + 1)][(4 * (_x + 1))] = ' ';
                                        layerMap[((4 * _y) + 2)][(4 * (_x + 1))] = ' ';
                                        layerMap[((4 * _y) + 3)][(4 * (_x + 1))] = ' ';
                                    }
                                }
                            }
                        }

                        //Create wallYs
                        for (int _x = 0; _x < x; _x++)
                        {
                            for (int _y = 0; _y < y; _y++)
                            {
                                if (_y != (y - 1))
                                {
                                    if (boundVals[((2 * _y) + 1)][(3 * _x)])
                                    {
                                        layerMap[(4 * (_y + 1))][((4 * _x) + 1)] = '#';
                                        layerMap[(4 * (_y + 1))][((4 * _x) + 2)] = '#';
                                        layerMap[(4 * (_y + 1))][((4 * _x) + 3)] = '#';
                                    }
                                    else
                                    {
                                        layerMap[(4 * (_y + 1))][((4 * _x) + 1)] = ' ';
                                        layerMap[(4 * (_y + 1))][((4 * _x) + 2)] = ' ';
                                        layerMap[(4 * (_y + 1))][((4 * _x) + 3)] = ' ';
                                    }
                                }
                            }
                        }

                        //Create corners
                        for (int _x = 0; _x < x; _x++)
                        {
                            for (int _y = 0; _y < y; _y++)
                            {
                                //Corner
                                if ((_x != (x - 1)) && (_y != (y - 1)))
                                {
                                    if (boundVals[((2 * _y) + 1)][((3 * _x) + 2)])
                                    {
                                        layerMap[(4 * (_y + 1))][(4 * (_x + 1))] = '#';
                                    }
                                    else
                                    {
                                        layerMap[(4 * (_y + 1))][(4 * (_x + 1))] = ' ';
                                    }
                                }
                                //Corner Up
                                if ((_x != (x - 1)) && (_y != 0))
                                {
                                    if (boundVals[((2 * _y) - 1)][((3 * _x) + 2)])
                                    {
                                        layerMap[(4 * _y)][(4 * (_x + 1))] = '#';
                                    }
                                    else
                                    {
                                        layerMap[(4 * _y)][(4 * (_x + 1))] = ' ';
                                    }
                                }
                                //Corner Left
                                if ((_x != 0) && (_y != (y - 1)))
                                {
                                    if (boundVals[((2 * _y) + 1)][((3 * _x) - 1)])
                                    {
                                        layerMap[(4 * (_y + 1))][(4 * _x)] = '#';
                                    }
                                    else
                                    {
                                        layerMap[(4 * (_y + 1))][(4 * _x)] = ' ';
                                    }
                                }
                            }
                        }

                        //Create lowers
                        for (int _x = 0; _x < x; _x++)
                        {
                            for (int _y = 0; _y < y; _y++)
                            {
                                if (boundVals[(2 * _y)][(3 * _x)])
                                 {
                                     layerMap[((4 * _y) + 2)][((4 * _x) + 1)] = ' ';
                                 }
                                 else
                                 {
                                     layerMap[((4 * _y) + 2)][((4 * _x) + 1)] = '-';
                                 }
                            }
                        }

                        //Create uppers
                        for (int _x = 0; _x < x; _x++)
                        {
                            for (int _y = 0; _y < y; _y++)
                            {
                                if (boundVals[(2 * _y)][((3 * _x) + 1)])
                                {
                                    layerMap[((4 * _y) + 2)][((4 * _x) + 3)] = ' ';
                                }
                                else
                                {
                                    layerMap[((4 * _y) + 2)][((4 * _x) + 3)] = '+';
                                }
                            }
                        }

                        for (int _z = z - 1; _z >= 0; _z--)
                        {
                            if (layout[_z] == null)
                            {
                                if (_z == 0)
                                {
                                    layout[_z] = layerMap;
                                }
                                else
                                {
                                    if (layout[_z - 1] != null)
                                    {
                                        layout[_z] = layerMap;
                                    }
                                }
                            }
                        }
            * */
        }

        /// <summary>
        /// 
        /// </summary>
        private void CreateStartEnd()
        {
            switch (rand.Next(0, 4))
            {
                case 0:
                    //Get start
                    startCoor = new int[] { rand.Next(0, (z - 1)), rand.Next(0, (y - 1)), 0 };
                    //Get end
                    endCoor = new int[] { rand.Next(0, (z - 1)), rand.Next(0, (y - 1)), (x - 1) };
                    break;
                case 1:
                    //Get start
                    startCoor = new int[] { rand.Next(0, (z - 1)), rand.Next(0, (y - 1)), (x - 1) };
                    //Get end
                    endCoor = new int[] { rand.Next(0, (z - 1)), rand.Next(0, (y - 1)), 0 };
                    break;
                case 2:
                    //Get start
                    startCoor = new int[] { rand.Next(0, (z - 1)), 0, rand.Next(0, (x - 1)) };
                    //Get end
                    endCoor = new int[] { rand.Next(0, (z - 1)), (y - 1), rand.Next(0, (x - 1)) };
                    break;
                case 3:
                    //Get start
                    startCoor = new int[] { rand.Next(0, (z - 1)), (y - 1), rand.Next(0, (x - 1)) };
                    //Get end
                    endCoor = new int[] { rand.Next(0, (z - 1)), 0, rand.Next(0, (x - 1)) };
                    break;
            }

            //get startCoorRange & playerCoor
            startCoorRange = new int[2][];
            if (startCoor[2] == 0)
            {
                startCoorRange[0] = new int[] { startCoor[0], UY_wallX(startCoor[1]), startCoor[2] };
                startCoorRange[1] = new int[] { startCoor[0], LY_wallX(startCoor[1]), (wallWidth - 1) };
                playerCoor = new int[] { startCoor[0], CY_wallX(startCoor[1]), startCoor[2] };
            }
            if (startCoor[2] == (x - 1))
            {
                startCoorRange[0] = new int[] { startCoor[0], UY_wallX(startCoor[1]), RX_wallX(startCoor[2]) };
                startCoorRange[1] = new int[] { startCoor[0], LY_wallX(startCoor[1]), (startCoorRange[0][2] + wallWidth - 1) };
                playerCoor = new int[] { startCoor[0], CY_wallX(startCoor[1]), RX_wallX(startCoor[2]) };
            }
            if (startCoor[1] == 0)
            {
                startCoorRange[0] = new int[] { startCoor[0], startCoor[1], LX_wallY(startCoor[2]) };
                startCoorRange[1] = new int[] { startCoor[0], (wallHeight - 1), RX_wallY(startCoor[2]) };
                playerCoor = new int[] { startCoor[0], startCoor[1], CX_wallY(startCoor[2]) };
            }
            if (startCoor[1] == (y - 1))
            {
                startCoorRange[0] = new int[] { startCoor[0], LY_wallY(startCoor[1]), LX_wallY(startCoor[2]) };
                startCoorRange[1] = new int[] { startCoor[0], (startCoorRange[0][1] + wallHeight - 1), RX_wallY(startCoor[2]) };
                playerCoor = new int[] { startCoor[0], LY_wallX(startCoor[1]), CX_wallY(startCoor[2]) };
            }
            //get endCoorRange
            endCoorRange = new int[2][];
            if (endCoor[2] == 0)
            {
                endCoorRange[0] = new int[]{endCoor[0], UY_wallX(endCoor[1]), endCoor[2]};
                endCoorRange[1] = new int[]{endCoor[0], LY_wallX(endCoor[1]), (wallWidth - 1)};
            }
            if (endCoor[2] == (x - 1))
            {
                endCoorRange[0] = new int[] { endCoor[0], UY_wallX(endCoor[1]), RX_wallX(endCoor[2]) };
                endCoorRange[1] = new int[] { endCoor[0], LY_wallX(endCoor[1]), (endCoorRange[0][2] + wallWidth - 1) };
            }
            if (endCoor[1] == 0)
            {
                endCoorRange[0] = new int[] { endCoor[0], endCoor[1], LX_wallY(endCoor[2]) };
                endCoorRange[1] = new int[] { endCoor[0], (wallHeight - 1), RX_wallY(endCoor[2]) };
            }
            if (endCoor[1] == (y - 1))
            {
                endCoorRange[0] = new int[] { endCoor[0], UY_wallY(endCoor[1]) , LX_wallY(endCoor[2]) };
                endCoorRange[1] = new int[] { endCoor[0], (endCoorRange[0][1] + wallHeight - 1), RX_wallY(endCoor[2]) };
            }
            //Add openings to grid
            for (int _y = startCoorRange[0][1]; _y <= startCoorRange[1][1]; _y++)
            {
                for (int _x = startCoorRange[0][2]; _x <= startCoorRange[1][2]; _x++)
                {
                    layout[startCoorRange[0][0]][_y][_x] = ' ';
                }
            }
            for (int _y = endCoorRange[0][1]; _y <= endCoorRange[1][1]; _y++)
            {
                for (int _x = endCoorRange[0][2]; _x <= endCoorRange[1][2]; _x++)
                {
                    layout[endCoorRange[0][0]][_y][_x] = ' ';
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_x"></param>
        /// <returns></returns>
        protected int LX_wallX(int _x)
        {
            return (unitWidth * _x);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_x"></param>
        /// <returns></returns>
        protected int RX_wallX(int _x)
        {
            return ((unitWidth * _x) + unitWidth);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_y"></param>
        /// <returns></returns>
        protected int UY_wallY(int _y)
        {
            return (unitHeight * _y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_y"></param>
        /// <returns></returns>
        protected int LY_wallY(int _y)
        {
            return ((unitHeight * _y) + unitHeight);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_y"></param>
        /// <returns></returns>
        protected int UY_wallX(int _y)
        {
            return ((unitHeight * _y) + wallHeight);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_y"></param>
        /// <returns></returns>
        protected int LY_wallX(int _y)
        {
            return ((unitHeight * _y) + unitHeight - 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_x"></param>
        /// <returns></returns>
        protected int LX_wallY(int _x)
        {
            return ((unitWidth * _x) + wallWidth);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_x"></param>
        /// <returns></returns>
        protected int RX_wallY(int _x)
        {
            return ((unitWidth * _x) + unitWidth - 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_y"></param>
        /// <returns></returns>
        protected int CY_wallX(int _y)
        {
            return ((unitHeight * _y) + ((wallHeight + unitHeight - 1) / 2));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_x"></param>
        /// <returns></returns>
        protected int CX_wallY(int _x)
        {
            return ((unitWidth * _x) + ((wallWidth + unitWidth - 1) / 2));
        }

        protected int[] Coor_upper(int _y, int _x, int[] lower)
        {
            int[] coor = new int[2];

            do
            {
                coor[0] = rand.Next(UY_wallX(_y), (LY_wallX(_y) + 1));
                coor[1] = rand.Next(LX_wallY(_x), (RX_wallY(_x) + 1));
            } while ((coor[0] == lower[0]) && (coor[1] == lower[1]));

            return coor;
        }

        /// <summary>
        /// A private nested class in <c>Maze</c> that uses cells to create each layer of the maze.
        /// <para>Changes are made to itself when a new layer is generated.</para>
        /// <para>Data from a <c>Layer</c> object can be gotten in the form of a 2D boolean array.</para>
        /// </summary>
        private class Layer
        {
            static Cell root;
            int x, y;
            static List<Cell> sets;

            public Layer(int _x, int _y)
            {
                x = _x;
                y = _y;
                sets = new List<Cell>();
                root = new Cell();
                CreateGrid();
            }

            void CreateGrid()
            {
                Cell hold = root;
                for (int _x = 1; _x < x; _x++)
                {
                    hold = hold.GetX();
                }

                hold = root;
                for (int _y = 1; _y < y; _y++)
                {
                    hold = hold.GetY();
                }
                CreateCeilings();
            }

            void CreateCeilings()
            {
                Cell hold;
                Cell[] _sets = sets.ToArray();
                foreach (Cell c in _sets)
                {
                    while (!c.HasOpenZ())
                    {
                        hold = c;
                        while (hold != null)
                        {
                            hold.WallZ();
                            hold = hold.setChild;
                        }
                    }
                }
            }

            internal void NextLayer()
            {
                ToNextLayer();
                CreateCeilings();
            }

            void ToNextLayer()
            {
                Cell[] _sets = sets.ToArray();
                Cell hold, holdChild;
                foreach (Cell c in _sets)
                {
                    hold = c;
                    holdChild = c.setChild;
                    while (hold != null)
                    {
                        hold.ToNextLayer();
                        if (holdChild != null)
                        {
                            hold = holdChild;
                            if (hold.setChild != null)
                            {
                                holdChild = hold.setChild;
                            }
                            else
                            {
                                holdChild = null;
                            }
                        }
                        else
                        {
                            hold = null;
                        }
                    }

                }
                getNewWalls();
            }

            void getNewWalls()
            {
                Cell holdX, holdY;
                holdY = root;
                while (holdY != null)
                {
                    holdX = holdY;
                    while (holdX != null)
                    {
                        holdX.WallX();
                        holdX = holdX.x;
                    }
                    holdX = holdY;
                    while (holdX != null)
                    {
                        holdX.WallY();
                        holdX = holdX.x;
                    }
                    holdY = holdY.y;
                }
            }

            internal void CompleteMaze()
            {
                ToNextLayer();
                while (sets.Count > 1)
                {
                    sets.ForEach(ForEachChildRemoveWalls);
                }
            }

            private static void RemoveWalls(Cell c)
            {
                Cell hold = c;
                switch (rand.Next(0, 2))
                {
                    case 0:
                        while (hold.x != null)
                        {
                            if (!hold.SameSet(hold.x))
                            {
                                hold.RemoveWallX();
                                break;
                            }
                            hold = hold.x;
                        }
                        break;

                    case 1:
                        while (hold.y != null)
                        {
                            if (!hold.SameSet(hold.y))
                            {
                                hold.RemoveWallY();
                                break;
                            }
                            hold = hold.y;
                        }
                        break;
                }
            }

            private static void ForEachChildRemoveWalls(Cell c)
            {
                Cell hold = c;
                while (hold != null)
                {
                    RemoveWalls(hold);
                    hold = hold.setChild;
                }
            }

            internal bool[][] GetBoundaryValues()
            {
                //Create 2d array of ints to represent a grid
                //Every cell {(x, y) | (x is real) ^ (y is real) ^ ((x % 3) == 0) ^ ((y % 2) == 0)} will contain bools for lower
                //Every cell {(x, y) | (x is real) ^ (y is real) ^ ((x % 3) == 1) ^ ((y % 2) == 0)} will contain bools for upper
                //Every cell {(x, y) | (x is real) ^ (y is real) ^ ((x % 3) == 2) ^ ((y % 2) == 0)} will contain bools for wallX
                //Every cell {(x, y) | (x is real) ^ (y is real) ^ (((x % 3) == 0) || ((x % 3) == 1)) ^ ((y % 2) == 1)} will contain bools for wallY
                //Every cell {(x, y) | (x is real) ^ (y is real) ^ ((x % 3) == 2) ^ ((y % 2) == 1)} will contain bools for corner
                bool[][] grid = new bool[(2 * y) - 1][];
                for (int i = 0; i < ((2 * y) - 1); i++)
                {
                    grid[i] = new bool[(3 * x) - 1];
                }
                int _x, _y;
                Cell holdY, holdX;
                holdY = root;
                _y = 0;


                //Getting values
                //((3*x),(2*y)) - lower
                //(((3*x)+1),(2*y)) - upper
                //If (holdX.x != null) : (((3*x)+2),(2*y)) - wallX
                //If (holdY.y != null) : ((3*x),((2*y)+1)) - wallY
                //If (holdY.y != null) : (((3*x)+1),((2*y)+1)) - wallY
                //If ((holdX.x != null) ^ (holdY.y != null)) : (((3*x)+2),((2*y)+1)) - corner (see Below)
                // corner =!(wallX || wallY)

                //Getting corners
                //If (holdX.x != null) ^ (holdY.y != null): corner = (!wallX ^ !wallY)
                //If (x != 0): corner = (corner ^ !((((3*x)-1),(2*y)) || ((3*x),((2*y)+1))) corner is (((3*x)-1),((2*y)+1))
                //If (y != 0): corner = (corner ^ !((((3*x)+2),(2*y)) || ((3*x),((2*y)-1))) corner is (((3*x)+2),((2*y)-1))

                while (holdY != null)
                {
                    holdX = holdY;
                    _x = 0;
                    while (holdX != null)
                    {
                        grid[boundY0(_y)][boundX0(_x)] = holdX.lower;
                        grid[boundY0(_y)][boundX1(_x)] = holdX.upper;
                        if (holdX.x != null)
                        {
                            grid[boundY0(_y)][boundX2(_x)] = holdX.wallX;
                        }
                        if (holdY.y != null)
                        {
                            grid[boundY1(_y)][boundX0(_x)] = holdX.wallY;
                            grid[boundY1(_y)][boundX1(_x)] = holdX.wallY;
                        }
                        //Getting corners
                        //If (holdX.x != null) ^ (holdY.y != null): corner = (wallX  || wallY)
                        if ((holdX.x != null) && (holdY.y != null))
                        {
                            grid[boundY1(_y)][boundX2(_x)] = (grid[boundY0(_y)][boundX2(_x)] || grid[boundY1(_y)][boundX0(_x)]);
                        }
                        //If ((x != 0) ^ (holdY.y != null)): corner = (corner ^ ((((3*x)-1),(2*y)) || ((3*x),((2*y)+1))) corner is (((3*x)-1),((2*y)+1))
                        //Get left corner
                        if ((_x != 0) && (holdY.y != null))
                        {
                            grid[boundY1(_y)][boundX2(_x - 1)] = (grid[boundY1(_y)][boundX2(_x - 1)] && (grid[boundY0(_y)][boundX2(_x - 1)] || grid[boundY1(_y)][boundX0(_x)]));
                        }
                        //If ((y != 0) ^ (holdX.x != null)): corner = (corner ^ ((((3*x)+2),(2*y)) || ((3*x),((2*y)-1))) corner is (((3*x)+2),((2*y)-1))
                        //Get upper corner
                        if ((_y != 0) && (holdX.x != null))
                        {
                            grid[boundY1(_y - 1)][boundX2(_x)] = (grid[boundY1(_y - 1)][boundX2(_x)] && (grid[boundY0(_y)][boundX2(_x)] || grid[boundY1(_y - 1)][boundX0(_x)]));
                        }
                        //Get upper-left corner
                        if ((_y != 0) && (_x != 0))
                        {
                            grid[boundY1(_y - 1)][boundX2(_x - 1)] = (grid[boundY1(_y - 1)][boundX2(_x - 1)] && (grid[boundY0(_y)][boundX2(_x - 1)] || grid[boundY1(_y - 1)][boundX0(_x)]));
                        }

                        holdX = holdX.x;
                        _x++;
                    }
                    holdY = holdY.y;
                    _y++;
                }

                //TESTING - WRITING TO CONSOLE
                /*
                for (int _ay = 0; _ay < grid.Length; _ay++)
                {
                    if (_ay == 0)
                    {
                        for (int _ax = 0; _ax < grid[_y].Length; _ax++)
                        {
                            if (_ax == 0)
                            {
                                Console.Write("#");
                            }
                            if ((_ax % 3) == 0)
                            {
                                Console.Write("##");
                            }
                            else
                            {
                                Console.Write("#");
                            }
                            if (_ax == (grid[_ay].Length - 1))
                            {
                                Console.Write("#");
                            }
                        }
                        Console.WriteLine();
                    }
                    if ((_ay % 2) == 0)
                    {
                        for (int hY = 0; hY < 3; hY++)
                        {
                            for (int _ax = 0; _ax < grid[_y].Length; _ax++)
                            {
                                if (_ax == 0)
                                {
                                    Console.Write("#");
                                }
                                if (grid[_ay][_ax])
                                {
                                    if (((_ax % 3) == 0))
                                    {
                                        Console.Write("  ");
                                    }
                                    if ((_ax % 3) == 1)
                                    {
                                        Console.Write(" ");
                                    }
                                    if ((_ax % 3) == 2)
                                    {
                                        Console.Write(((_ax-2)/3));
                                    }
                                }
                                else
                                {
                                    if (((_ax % 3) == 0))
                                    {
                                        Console.Write("- ");
                                    }
                                    if ((_ax % 3) == 1)
                                    {
                                        Console.Write("+");
                                    }
                                    if ((_ax % 3) == 2)
                                    {
                                        Console.Write(" ");
                                    }
                                }
                                if (_ax == (grid[_ay].Length - 1))
                                {
                                    Console.Write("#");
                                }
                            }
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        for (int _ax = 0; _ax < grid[_y].Length; _ax++)
                        {
                            if (_ax == 0)
                            {
                                Console.Write("#");
                            }
                            if (grid[_ay][_ax])
                            {
                                if (((_ax % 3) == 0))
                                {
                                    Console.Write(((_ay- 1) / 2));
                                    Console.Write(((_ay - 1) / 2));
                                }
                                if ((_ax % 3) == 1)
                                {
                                    Console.Write(((_ay - 1) / 2));
                                }
                                if ((_ax % 3) == 2)
                                {
                                    Console.Write("X");
                                }
                            }
                            else
                            {
                                if (((_ax % 3) == 0))
                                {
                                    Console.Write("  ");
                                }
                                if ((_ax % 3) == 1)
                                {
                                    Console.Write(" ");
                                }
                                if ((_ax % 3) == 2)
                                {
                                    Console.Write(" ");
                                }
                            }
                            if (_ax == (grid[_ay].Length - 1))
                            {
                                Console.Write("#");
                            }
                        }
                        Console.WriteLine();
                    }
                    if (_ay == (grid.Length - 1))
                    {
                        for (int _ax = 0; _ax < grid[_y].Length; _ax++)
                        {
                            if (_ax == 0)
                            {
                                Console.Write("#");
                            }
                            if ((_ax % 3) == 0)
                            {
                                Console.Write("##");
                            }
                            else
                            {
                                Console.Write("#");
                            }
                            if (_ax == (grid[_ay].Length - 1))
                            {
                                Console.Write("#");
                            }
                        }
                        Console.WriteLine();
                    }
                }
                 * */
                //END TESTING

                return grid;
            }

            /// <summary>
            /// A private nested class in <c>Layer</c> that defines 'cells'.
            /// <para>Defines if they have right (<c>wallX</c>), down (<c>wallY</c>), upper (<c>upper</c>), and lower (<c>lower</c>) barriers.</para>
            /// <para>Cells also link to the cells their right (<c>x</c>) and down (<c>y</c>), as well as their set parent (<c>setParent</c>) and set child (<c>setChild</c>).</para>
            /// </summary>
            private class Cell
            {
                internal bool wallX, wallY, upper, lower;
                internal Cell x, y, setParent, setChild;

                internal Cell()
                {
                    wallX = false;
                    wallY = false;
                    upper = true;
                    lower = true;
                    x = null;
                    y = null;
                    NewSet();
                }

                private void NewSet()
                {
                    setParent = null;
                    setChild = null;
                    if (!sets.Contains(this))
                    {
                        sets.Add(this);
                    }
                }

                private void NewSet(Cell parent)
                {
                    Cell holdP, holdC;
                    holdP = parent;
                    holdC = this;

                    while (holdP.setChild != null)
                    {
                        holdP = holdP.setChild;
                    }
                    while (holdC.setParent != null)
                    {
                        holdC = holdC.setParent;
                    }

                    sets.Remove(holdC);
                    holdP.setChild = holdC;
                    holdC.setParent = holdP;
                }

                internal void NextLayerSet()
                {
                    if (setParent != null)
                    {
                        if (setParent.lower)
                        {
                            setParent = null;
                            if (!sets.Contains(this))
                            {
                                sets.Add(this);
                            }
                        }
                    }
                    while (setChild != null)
                    {
                        if (!setChild.upper)
                        {
                            setChild.setParent = this;
                            break;
                        }
                        setChild = setChild.setChild;
                    }
                }

                internal bool SameSet(Cell c)
                {
                    Cell holdC, holdP;
                    holdC = c;
                    holdP = this;

                    while (holdC.setParent != null)
                    {
                        if (holdC.setParent == c)
                        {
                            Console.WriteLine("c has itself has parent");
                            Console.ReadLine();
                        }
                       

                        if (holdC.setParent == this)
                        {
                            return true;
                        }
                        holdC = holdC.setParent;
                    }
                    holdC = c;
                    while (holdC.setChild != null)
                    {
                        if (holdC.setChild == this)
                        {
                            return true;
                        }
                        holdC = holdC.setChild;
                    }
                    return false;

                    /*
                    while (holdC.setParent != null)
                    {
                        holdC = holdC.setParent;
                    }
                    while (holdP.setParent != null)
                    {
                        holdC = holdP.setParent;
                    }
                    if (holdC == holdP)
                    {
                        return true;
                    }
                    return false;
                     * */
                }

                internal void WallX()
                {
                    wallX = (rand.Next(100) % 2 != 0);
                    if (x != null)
                    {
                        wallX = (wallX || SameSet(x));
                        if (!wallX)
                        {
                            x.NewSet(this);
                        }
                    }
                }

                internal void WallY()
                {
                    wallY = (rand.Next(100) % 2 != 0);
                    if (y != null)
                    {
                        wallY = (wallY || SameSet(y));
                        if (!wallY)
                        {
                            y.NewSet(this);
                        }
                    }
                }

                internal void RemoveWallX()
                {
                    if (x != null)
                    {
                        if (!SameSet(x))
                        {
                            wallX = false;
                            x.NewSet(this);
                        }
                    }
                }

                internal void RemoveWallY()
                {
                    if (y != null)
                    {
                        if (!SameSet(y))
                        {
                            wallY = false;
                            y.NewSet(this);
                        }
                    }
                }

                internal void WallZ()
                {
                    upper = (rand.Next(100) % 2 != 0);
                    /*
                    upper = ((rand.Next(100) % 2 != 0) && (setParent != null || setChild != null));
                    if (setParent != null)
                    {
                        if (setChild != null)
                        {
                            upper = (upper && setParent.ParentZ(!upper || setChild.ChildZ(!upper)));
                        }
                        else
                        {
                            upper = (upper && setParent.ParentZ(!upper));
                        }
                    }
                    else
                    {
                        if (setChild != null)
                        {
                            upper = (upper && setChild.ChildZ(!upper));
                        }
                    }
                     * */
                }

                //To be used on the first cell in a set and recursively on child cells
                //Returns if the set of cells has a opening in the ceiling at some point
                internal bool HasOpenZ()
                {
                    if (!upper)
                    {
                        return !upper;
                    }

                    if (setChild != null)
                    {
                        return setChild.HasOpenZ();
                    }

                    return !upper;
                }

                internal void CloseWallZ()
                {
                    upper = true;
                }

                internal Cell GetX()
                {
                    if (x == null)
                    {
                        x = new Cell();
                        WallX();
                    }

                    return x;
                }

                /// <summary>
                /// 
                /// </summary>
                /// <returns></returns>
                internal Cell GetY()
                {
                    if (y == null)
                    {
                        y = new Cell();
                        WallY();
                        if (x != null)
                        {
                            y.x = x.GetY();
                            y.WallX();
                        }
                    }
                    return y;
                }

                internal void ToNextLayer()
                {
                    wallX = false;
                    wallY = false;
                    lower = upper;
                    upper = true;
                    if (!lower)
                    {
                        NextLayerSet();
                    }
                    else
                    {
                        NewSet();
                    }
                }
            }
        }
    }
}
