using Utils;

namespace MazeRunner
{
    public struct Direction
    {
        public Heading Heading { get; set; }
        public int Cost { get; set; }

        public Direction(
            Heading heading,
            int cost)
        {
            Heading = heading;
            Cost = cost;
        }
    }
}
