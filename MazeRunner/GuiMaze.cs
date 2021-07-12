using System;
using System.Collections.Generic;
using Utils;

namespace MazeRunner
{
    public class GuiMaze
    {
        private readonly Runner _runner;
        private readonly string[,] _map;
        private readonly HashSet<(int, int)> _shortestPath = new HashSet<(int, int)>();

        public GuiMaze(
            Runner runner)
        {
            _runner = runner;
            _map = new string[_runner.Rows, _runner.Cells];
            InitMap();
        }

        private void InitMap()
        {
            for (int y = 0; y < _runner.Rows; y++)
            {
                for (int x = 0; x < _runner.Cells; x++)
                {
                    _map[y, x] = "#";
                }
            }
        }

        private void FillMap(Node node)
        {
            _map[node.Position.Y, node.Position.X] = node.Value.ToString();
            foreach (var route in node.Routes)
            {
                if (route == null)
                {
                    continue;
                }

                int y = node.Position.Y;
                int x = node.Position.X;

                foreach (var direction in route.Path)
                {
                    Move(ref y, ref x, direction.Heading);
                    _map[y, x] = direction.Cost.ToString();
                }
            }
        }

        public void Draw(Node node)
        {
            if (Console.KeyAvailable)
            {
                Console.Clear();
                _shortestPath.Clear();

                FillMap(node);

                int _y = node.Position.Y;
                int _x = node.Position.X;
                var currentNode = node;
                while (true)
                {
                    var parent = currentNode.Parent;
                    if (parent == null)
                    {
                        _shortestPath.Add((currentNode.Position.Y, currentNode.Position.X));
                        break;
                    }

                    _y = parent.Position.Y;
                    _x = parent.Position.X;

                    var directions = NodeHelper.GetPathBetweenNodes(parent, currentNode);
                    foreach (var direction in directions)
                    {
                        Move(ref _y, ref _x, direction.Heading);
                        _shortestPath.Add((_y, _x));
                    }

                    currentNode = parent;
                }

                for (int y = 0; y < _runner.Rows; y++)
                {
                    for (int x = 0; x < _runner.Cells; x++)
                    {
                        if (_shortestPath.Contains((y, x)))
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                        Console.Write(_map[y, x]);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.WriteLine();
                }
                Console.ReadKey();
            }
        }

        private void Move(ref int y, ref int x, Heading heading)
        {
            if (heading == Heading.N)
            {
                y -= 1;
            }
            else if (heading == Heading.E)
            {
                x += 1;
            }
            else if (heading == Heading.S)
            {
                y += 1;
            }
            else if (heading == Heading.W)
            {
                x -= 1;
            }
        }
    }
}
