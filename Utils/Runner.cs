using System;
using System.IO;

namespace Utils
{
    public class Runner
    {
        internal int[,] Map { get; private set; }

        public int Rows { get; private set; }
        public int Cells { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }

        public Runner(string path)
        {
            LoadMap(path);
        }

        private void LoadMap(string path)
        {
            if (!File.Exists(path))
            {
                throw new Exception($"File does not exist: {path}");
            }

            var lines = File.ReadAllLines(path);
            Rows = lines.Length;
            Cells = lines[0].Length;
            Map = new int[Rows, Cells];

            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    var current = lines[y][x];
                    if (current == 'B')
                    {
                        Map[y, x] = CellType.Start;
                        X = x;
                        Y = y;
                    }
                    else if (current == 'T')
                    {
                        Map[y, x] = CellType.Treasure;
                    }
                    else if (current == '#')
                    {
                        Map[y, x] = CellType.Wall;
                    }
                    else
                    {
                        Map[y, x] = current - '0';
                    }
                }
            }
        }

        public bool Move(Heading heading)
        {
            if (heading == Heading.N && Y > 0 && Map[Y-1, X] != CellType.Wall)
            {
                Y -= 1;
                return true;
            }

            if (heading == Heading.E && X < Cells - 1 && Map[Y, X+1] != CellType.Wall)
            {
                X += 1;
                return true;
            }

            if (heading == Heading.S && Y < Rows - 1 && Map[Y+1, X] != CellType.Wall)
            {
                Y += 1;
                return true;
            }

            if (heading == Heading.W && X > 0 && Map[Y, X-1] != CellType.Wall)
            {
                X -= 1;
                return true;
            }

            return false;
        }

        public int[,] Scan()
        {
            var result = new int[3, 3];
            for (int y = Y - 1, _y = 0; y <= Y + 1; y++, _y++)
            {
                for (int x = X - 1, _x = 0; x <= X + 1; x++, _x++)
                {
                    if (y >= 0 && y < Rows && x >= 0 && x < Cells)
                    {
                        result[_y, _x] = Map[y, x];
                    }
                    else
                    {
                        result[_y, _x] = CellType.Wall;
                    }
                }
            }
            return result;
        }
    }
}
