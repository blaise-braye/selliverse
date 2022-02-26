using System;
using System.Numerics;

namespace Selliverse.Server
{
    public class ThrottledPosition
    {
        private static readonly double epsilon = .00001;

        public Vector3 Position { get; set; }

        public bool HasUpdated { get; set; }


        public static ThrottledPosition Update(ThrottledPosition lastPosition, Vector3 newPosition)
        {
            if (Math.Abs(newPosition.X - lastPosition.Position.X) < epsilon &&
                Math.Abs(newPosition.Y - lastPosition.Position.Y) < epsilon &&
                Math.Abs(newPosition.Z - lastPosition.Position.Z) < epsilon)
            {
                return new ThrottledPosition
                {
                    Position = newPosition,
                    HasUpdated = false
                };
            }
            else
            {
                return new ThrottledPosition
                {
                    Position = newPosition,
                    HasUpdated = true
                };
            }
        }
    }
}
