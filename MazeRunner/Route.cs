using System.Collections.Generic;

namespace MazeRunner
{
    public class Route
    {
        public List<Direction> Path { get; set; }
        public Node Src { get; set; }
        public Node Dest { get; set; }
    }
}
