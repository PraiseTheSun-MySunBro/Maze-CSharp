using System;

namespace Utils
{
    public static class HeadingHelper
    {
        public static Heading GetReversedHeading(Heading heading)
        {
            return heading switch
            {
                Heading.N => Heading.S,
                Heading.E => Heading.W,
                Heading.S => Heading.N,
                Heading.W => Heading.E,
                _ => throw new NotImplementedException($"Heading {heading} is not supported")
            };
        }

        public static int GetDirectionAsInteger(Heading heading)
        {
            return (int)heading;
        }
    }
}
