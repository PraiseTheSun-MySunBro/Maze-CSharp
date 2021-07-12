using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace MazeRunner
{
    public class Solver
    {
        private readonly Dictionary<Position, Node> _nodes = new Dictionary<Position, Node>();
        private readonly HashSet<Node> _visited = new HashSet<Node>();
        private readonly MinPriorityQueue<Node> _pq = new MinPriorityQueue<Node>();
        private readonly Runner _runner;
        private readonly GuiMaze _guiMaze;

        public Position Start { get; private set; }
        public Position End { get; private set; }

        public Solver(
            Runner runner,
            GuiMaze guiMaze = null)
        {
            _runner = runner;
            _guiMaze = guiMaze;
        }

        public IList<Heading> Solve()
        {
            if (_runner.Rows == 0 || _runner.Cells == 0)
            {
                Console.WriteLine("Empty map");
                return new List<Heading>();
            }

            var rootPos = GetCurrentPosition();
            var root = new Node(rootPos, 0, null);
            Start = rootPos;
            AddNodeToQueue(0, root, rootPos);

            while (!_pq.IsEmpty())
            {
                var parentNode = _pq.Dequeue();
                MoveToNode(parentNode);

                if (parentNode.Position == End)
                {
                    Console.WriteLine($"Found treasure at {parentNode.Position}");
                    var distance = CalculateDistance(parentNode, new List<Direction>());
                    Console.WriteLine($"Distance = {distance}");
                    //_guiMaze.Draw(parentNode);
                    return new List<Heading>();
                }

                var directions = GetPossibleDirections(parentNode);
                if (directions.Count == 0)
                {
                    continue;
                }

                foreach (var direction in directions)
                {
                    var directionsToNextNode = MoveToNextNode(direction);
                    var nodePos = GetCurrentPosition();

                    if (!_nodes.ContainsKey(nodePos))
                    {
                        var cell = _runner.Scan()[1, 1];
                        if (cell == CellType.Treasure)
                        {
                            cell = 0;
                            End = nodePos;
                        }
                        var childNode = new Node(nodePos, cell, parentNode);
                        AddNodeToQueue(CalculateDistance(parentNode, directionsToNextNode), childNode, nodePos);
                        AddRouteToNextNode(parentNode, childNode, directionsToNextNode);
                    }
                    MoveBack(directionsToNextNode);
                }

                //_guiMaze.Draw(parentNode);
                _visited.Add(parentNode);
            }

            return new List<Heading>();
        }

        private void MoveToNode(Node dest)
        {
            static LinkedList<Node> CombineNodes(Node node)
            {
                var output = new LinkedList<Node>();
                output.AddLast(node);

                while (node.Parent != null)
                {
                    output.AddFirst(node.Parent);
                    node = node.Parent;
                }

                return output;
            }

            void MoveTo(Node src, Node dest)
            {
                foreach (var dir in NodeHelper.GetPathBetweenNodes(src, dest))
                {
                    _runner.Move(dir.Heading);
                }
            }

            var src = _nodes[GetCurrentPosition()];
            var srcNodes = CombineNodes(src);
            var destNodes = CombineNodes(dest);
            var destNodesAsSet = destNodes.ToHashSet();

            if (src == dest)
            {
                return;
            }

            while (true)
            {
                var srcNode = srcNodes.Last.Value;
                if (!destNodesAsSet.Contains(srcNode))
                {
                    srcNodes.RemoveLast();
                    var path = NodeHelper.GetPathBetweenNodes(srcNode.Parent, srcNode);
                    MoveBack(path);
                    continue;
                }

                while (true)
                {
                    var destNode = destNodes.First.Value;
                    if (srcNode != destNode)
                    {
                        destNodes.RemoveFirst();
                        continue;
                    }
                    destNodes.RemoveFirst();
                    break;
                }
                break;
            }

            while (destNodes.Count != 0)
            {
                var currentNode = srcNodes.Last.Value;
                var nextNode = destNodes.First.Value;
                MoveTo(currentNode, nextNode);

                srcNodes.AddLast(nextNode);
                destNodes.RemoveFirst();
            }
        }

        private void AddNodeToQueue(int priority, Node node, Position position)
        {
            _nodes.Add(position, node);
            _pq.Enqueue(priority, node);
        }

        private void AddRouteToNextNode(Node parent, Node child, List<Direction> directions)
        {
            var route = new Route
            {
                Path = directions,
                Src = parent,
                Dest = child
            };

            parent.Routes[GetDirectionAsInteger(directions[0])] = route;
        }

        private Position GetCurrentPosition()
        {
            return new Position(_runner.X, _runner.Y);
        }

        private int GetCurrentCell()
        {
            return _runner.Scan()[1, 1];
        }

        private List<Direction> GetPossibleDirections(Node node)
        {
            if (_visited.Contains(node))
            {
                return new List<Direction>();
            }

            var directions = new List<Direction>();
            var map = _runner.Scan();

            void AddDirection(Heading heading, int cell)
            {
                if (cell != CellType.Wall)
                {
                    directions.Add(new Direction(heading, cell));
                }
            }

            AddDirection(Heading.N, map[0, 1]);
            AddDirection(Heading.E, map[1, 2]);
            AddDirection(Heading.S, map[2, 1]);
            AddDirection(Heading.W, map[1, 0]);

            return directions;
        }

        private List<Direction> GetPossibleDirections(Heading prevHeading)
        {
            var directions = new List<Direction>();
            var map = _runner.Scan();

            void AddDirection(Heading heading, int cell)
            {
                if (HeadingHelper.GetReversedHeading(prevHeading) == heading)
                {
                    return;
                }

                if (cell != CellType.Wall)
                {
                    directions.Add(new Direction
                    {
                        Heading = heading,
                        Cost = cell
                    });
                }
            }

            AddDirection(Heading.N, map[0, 1]);
            AddDirection(Heading.E, map[1, 2]);
            AddDirection(Heading.S, map[2, 1]);
            AddDirection(Heading.W, map[1, 0]);

            return directions;
        }

        private int CalculateDistance(Node parent, List<Direction> directions)
        {
            int distance = 0;
            var node = parent;
            while (node?.Parent != null)
            {
                var src = node.Parent;
                var dest = node;

                distance += NodeHelper.GetPathBetweenNodes(src, dest).Sum(dir => dir.Cost);
                node = node.Parent;
            }

            foreach (var dir in directions)
            {
                if (dir.Cost < 0)
                {
                    continue;
                }

                distance += dir.Cost;
            }
            return distance;
        }

        private LinkedList<Node> GetNodesFromStart(Node current)
        {
            var nodes = new LinkedList<Node>();
            var node = current;
            while (node?.Parent != null)
            {
                nodes.AddFirst(node);
                node = node.Parent;
            }
            nodes.AddFirst(node);
            return nodes;
        }

        private int GetDirectionAsInteger(Direction direction)
        {
            return HeadingHelper.GetDirectionAsInteger(direction.Heading);
        }

        private List<Direction> MoveToNextNode(Direction direction)
        {
            var path = new List<Direction> { direction };
            var headingMoveTo = direction.Heading;

            while (true)
            {
                if (!_runner.Move(headingMoveTo))
                {
                    throw new ArgumentException($"Cannot move to {headingMoveTo}");
                }

                if (GetCurrentCell() == CellType.Treasure)
                {
                    return path;
                }

                var prevHeading = headingMoveTo;
                var directions = GetPossibleDirections(prevHeading);
                if (directions.Count == 1)
                {
                    var dir = directions[0];
                    headingMoveTo = dir.Heading;
                    dir.Cost = Math.Max(0, dir.Cost);
                    path.Add(dir);
                    continue;
                }
                return path;
            }
        }

        private void MoveBack(List<Direction> dirs)
        {
            var directions = new List<Direction>(dirs);
            while (directions.Count != 0)
            {
                var idx = directions.Count - 1;
                var heading = HeadingHelper.GetReversedHeading(directions[idx].Heading);
                _runner.Move(heading);
                directions.RemoveAt(idx);
            }
        }
    }
}
