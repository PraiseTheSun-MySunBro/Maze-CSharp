namespace MazeRunner
{
    public class Node
    {
        public Position Position { get; set; }
        public int Value { get; set; }
        public Route[] Routes { get; }
        public Node Parent { get; set; }

        public Node(
            Position position,
            int value,
            Node parent)
        {
            Position = position;
            Value = value;
            Parent = parent;
            Routes = new Route[4];
        }

        public override string ToString()
        {
            return $"Position: {Position} | Value: {Value}";
        }

        public override bool Equals(object obj)
        {
            return obj is Node node && this == node;
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }
    }
}
