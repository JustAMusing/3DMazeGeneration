﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _3DMazeGeneration
{
    class Program
    {
        static Maze maze;
        static MazeRunner mazeRunner;
        static readonly int windowWidth = 41;
        static readonly int windowHeight = 50;

        static void Main(string[] args)
        {
            Console.Title = "3D Maze Runner";
            Console.SetWindowSize(windowWidth, windowHeight);
            Console.SetBufferSize(windowWidth, windowHeight);

            do
            {
                Console.Clear();
                Start();
            } while (Reset());
          
        }

        static void Start()
        {
            int x = -1;
            int y = -1;
            int z = -1;
            int cW = -1;
            int cH = -1;
            int wW = -1;
            int wH = -1;

            Console.WriteLine("Please input:");
            while (y <= 0)
            {
                try
                {
                    Console.Write(" Length: ");
                    y = Convert.ToInt32(Console.ReadLine());
                    if (y <= 0)
                    {
                        Console.WriteLine("Please input a positive numerical value.");
                    }
                }
                catch (FormatException e)
                {
                    Console.WriteLine("Please input a positive numerical value.");
                } 
            }
            while (x <= 0)
            {
                try
                {
                    Console.Write(" Width: ");
                    x = Convert.ToInt32(Console.ReadLine());
                    if (x <= 0)
                    {
                        Console.WriteLine("Please input a positive numerical value.");
                    }
                }
                catch (FormatException e)
                {
                    Console.WriteLine("Please input a positive numerical value.");
                }
            }
            while (z <= 0)
            {
                try
                {
                    Console.Write(" Height: ");
                    z = Convert.ToInt32(Console.ReadLine());
                    if (z <= 0)
                    {
                        Console.WriteLine("Please input a positive numerical value.");
                    }
                }
                catch (FormatException e)
                {
                    Console.WriteLine("Please input a positive numerical value.");
                }
            }
            while (cW <= 0)
            {
                try
                {
                    Console.Write(" Cell Width: ");
                    cW = Convert.ToInt32(Console.ReadLine());
                    if (cW <= 0)
                    {
                        Console.WriteLine("Please input a positive numerical value.");
                    }
                }
                catch (FormatException e)
                {
                    Console.WriteLine("Please input a positive numerical value.");
                }
            }
            while (cH <= 0)
            {
                try
                {
                    Console.Write(" Cell Height: ");
                    cH = Convert.ToInt32(Console.ReadLine());
                    if (cH <= 0)
                    {
                        Console.WriteLine("Please input a positive numerical value.");
                    }
                }
                catch (FormatException e)
                {
                    Console.WriteLine("Please input a positive numerical value.");
                }
            }
            while (wW <= 0)
            {
                try
                {
                    Console.Write(" Wall Width: ");
                    wW = Convert.ToInt32(Console.ReadLine());
                    if (wW <= 0)
                    {
                        Console.WriteLine("Please input a positive numerical value.");
                    }
                }
                catch (FormatException e)
                {
                    Console.WriteLine("Please input a positive numerical value.");
                }
            }
            while (wH <= 0)
            {
                try
                {
                    Console.Write(" Wall Height: ");
                    wH = Convert.ToInt32(Console.ReadLine());
                    if (wH <= 0)
                    {
                        Console.WriteLine("Please input a positive numerical value.");
                    }
                }
                catch (FormatException e)
                {
                    Console.WriteLine("Please input a positive numerical value.");
                }
            }

            maze = new Maze(x, y, z, cW, cH, wW, wH);

            mazeRunner = new MazeRunner(maze);

            Console.WriteLine("\nPress [enter] to begin.\n");

            Console.Clear();
            mazeRunner.Run();
        }

        static bool Reset()
        {
            Console.WriteLine("Would you like to play again (Y/N)?");
            char input = Console.ReadKey().KeyChar;
            return (input == 'y');
        }
    }
}
