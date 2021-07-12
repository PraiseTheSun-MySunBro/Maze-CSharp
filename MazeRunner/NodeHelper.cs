using System;
using System.Collections.Generic;

namespace MazeRunner
{
    public static class NodeHelper
    {
        public static List<Direction> GetPathBetweenNodes(Node parent, Node child)
        {
            foreach (var route in parent.Routes)
            {
                if (route == null)
                {
                    continue;
                }

                if (route.Dest == child)
                {
                    return route.Path;
                }
            }

            throw new Exception($"Cannot find path between {parent} -> {child} nodes");
        }
    }
}
