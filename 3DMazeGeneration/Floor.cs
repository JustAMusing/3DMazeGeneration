using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace _3DMazeGeneration
{
    class Floor
    {
        static Random rand = new Random();
        Cell root;
        int x, y;
        static List<Cell> sets;

        public Floor(int _x, int _y)
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

        //May be able to get rid of
        void CloseCeilings()
        {
            Cell holdX, holdY;
            holdY = root;
            while (holdY != null)
            {
                holdX = holdY;
                while (holdX != null)
                {
                    holdX.CloseWallZ();
                    holdX = holdX.x;
                }
                holdY = holdY.y;
            }
        }

        internal void CompleteMaze()
        {
            Cell[] _sets;
            Cell hold;

            ToNextLayer();

            while (sets.Count > 1)
            {
                _sets = sets.ToArray();
                foreach (Cell c in _sets)
                {
                    hold = c;
                    switch (rand.Next(0,1))
                    {
                        case 0:
                            while(hold.x != null)
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
                            while(hold.y != null)
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
            }
        }

        //This isn't working right, check corner getting I think? -sigh-
        internal bool[][] GetGrid()
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
                    grid[(2 * _y)][(3 * _x)] = holdX.lower;
                    grid[(2 * _y)][((3 * _x) + 1)] = holdX.upper;
                    if (holdX.x != null)
                    {
                        grid[(2 * _y)][((3 * _x) + 2)] = holdX.wallX;
                    }
                    if (holdY.y != null)
                    {
                        grid[((2 * _y) + 1)][(3 * _x)] = holdX.wallY;
                        grid[((2 * _y) + 1)][((3 * _x) + 1)] = holdX.wallY;
                    }

                    //Getting corners
                    //If (holdX.x != null) ^ (holdY.y != null): corner = (wallX  || wallY)
                    if ((holdX.x != null) && (holdY.y != null))
                    {
                        grid[((2 * _y) + 1)][((3 * _x) + 2)] = (grid[(2 * _y)][((3 * _x) + 2)] || grid[((2 * _y) + 1)][(3 * _x)]);
                    }
                    //If ((x != 0) ^ (holdY.y != null)): corner = (corner ^ ((((3*x)-1),(2*y)) || ((3*x),((2*y)+1))) corner is (((3*x)-1),((2*y)+1))
                    if ((_x != 0) && (holdY.y != null))
                    {
                        grid[((2 * _y) + 1)][((3 * _x) - 1)] = (grid[((2 * _y) + 1)][((3 * _x) - 1)] && (grid[(2 * _y)][((3 * _x) - 1)] || grid[((2 * _y) + 1)][(3 * _x)]));
                    }
                    //If ((y != 0) ^ (holdX.x != null)): corner = (corner ^ ((((3*x)+2),(2*y)) || ((3*x),((2*y)-1))) corner is (((3*x)+2),((2*y)-1))

                    if ((_y != 0) && (holdX.x != null))
                    {
                        grid[((2 * _y) - 1)][((3 * _x) + 2)] = (grid[((2 * _y) - 1)][((3 * _x) + 2)] && (grid[(2 * _y)][((3 * _x) + 2)] || grid[((2 * _y) - 1)][(3 * _x)]));
                    }

                    holdX = holdX.x;
                    _x++;
                }
                holdY = holdY.y;
                _y++;
            }
            return grid;
        }

        //TESTING METHOD, should work fine now
        internal void WriteGrid()
        {
            bool[][] array = GetGrid();
            
            for (int _y = 0; _y < ((2*y)-1); _y++)
            {
                for (int _x = 0; _x < x; _x++)
                {
                    if ((_y % 2) == 0)
                    {
                        if (array[_y][(3 * _x)])
                        {
                            Console.Write(" ");
                        }
                        else
                        {
                            Console.Write("-");
                        }
                        if (array[_y][((3 * _x) + 1)])
                        {
                            Console.Write(" ");
                        }
                        else
                        {
                            Console.Write("+");
                        }
                        if (_x != x - 1)
                        {
                            if (array[_y][((3 * _x) + 2)])
                            {
                                Console.Write("#");
                            }
                            else
                            {
                                Console.Write(" ");
                            }
                        }
                    }
                    else
                    {
                        if (array[_y][(3 * _x)])
                        {
                            Console.Write("#");
                        }
                        else
                        {
                            Console.Write(" ");
                        }
                        if (array[_y][((3 * _x) + 1)])
                        {
                            Console.Write("#");
                        }
                        else
                        {
                            Console.Write(" ");
                        }
                        if (_x != x - 1)
                        {
                            if (array[_y][((3 * _x) + 2)])
                            {
                                Console.Write("#");
                            }
                            else
                            {
                                Console.Write(" ");
                            }
                        }
                    }
                }
                Console.WriteLine();
            }
        }

        //more godaamned testing
        internal void WriteInfo()
        {
            Console.WriteLine("---INFO---");
            Console.WriteLine("# of sets:"+sets.Count);
            Console.WriteLine();
            WriteGrid();
            Console.WriteLine();
            Console.WriteLine("------");
        }

        class Cell
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

            internal void NewSet()
            {
                setParent = null;
                setChild = null;
                if (!sets.Contains(this))
                {
                    sets.Add(this);
                }
            }

            public void NewSet(Cell parent)
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

            //REVISE REVISE REVISE
            public void NextLayerSet()
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
                    if (holdC.setParent == this)
                    {
                        return true;
                    }
                    holdC= holdC.setParent;
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

            /*
            bool ParentZ(bool open)
            {
                upper = ((rand.Next(100) % 2 != 0) && (open || setParent != null));
                if (setParent != null)
                {
                    upper = (upper && setParent.ParentZ(!upper || open));
                }
                return (!upper || open);
            }

            bool ChildZ(bool open)
            {
                upper = ((rand.Next(100) % 2 != 0) && (open || setChild != null));
                if (setChild != null)
                {
                    upper = (upper && setChild.ChildZ(!upper || open));
                }
                return (!upper || open);
            }
            */

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
