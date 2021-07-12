using System;
using System.Diagnostics;
using Utils;

namespace MazeRunner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var runner = new Runner(@"Maps/2000.maze");
            var guiMaze = new GuiMaze(runner);
            var timer = Stopwatch.StartNew();

            var solver = new Solver(runner, guiMaze);
            var shortestPath = solver.Solve();
            timer.Stop();

            Console.WriteLine($"The shortest path consists of {shortestPath.Count} steps");
            Console.WriteLine($"Total time elapsed: {timer.Elapsed}");

            Console.ReadLine();
        }
    }
}
