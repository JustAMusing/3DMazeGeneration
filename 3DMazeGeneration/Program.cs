using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _3DMazeGeneration
{
    class Program
    {
        static Maze maze;
        static MazeRunner mazeRunner;

        static void Main(string[] args)
        {
            do
            {
                Console.Clear();
                Start();
            } while (Reset());
          
        }

        static void Start()
        {
            int x, y, z;

            Console.WriteLine("Please input the maze's...");
            Console.Write(" Length: ");
            y = Convert.ToInt32(Console.ReadLine());
            Console.Write(" Width: ");
            x = Convert.ToInt32(Console.ReadLine());
            Console.Write(" Height: ");
            z = Convert.ToInt32(Console.ReadLine());

            maze = new Maze(x, y, z);
            mazeRunner = new MazeRunner(maze);

            Console.WriteLine();
            Console.WriteLine("Press [enter] to begin.");
            Console.ReadLine();

            Console.Clear();
            mazeRunner.Run();
        }

        static bool Reset()
        {
            char input;
            Console.WriteLine("Would you like to play again (Y/N)?");
            input = Console.ReadKey().KeyChar;
            return (input == 'y');
        }
    }
}
