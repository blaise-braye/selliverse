using System.Numerics;

namespace Selliverse.Server
{
    public class ThrottledPosition
    {
        public Vector3 Position { get; set; }

        public bool HasUpdated { get; set; }


        public static ThrottledPosition Update(ThrottledPosition lastPosition, Vector3 newPosition)
        {
            if (newPosition.X == lastPosition.Position.X &&
                newPosition.Y == lastPosition.Position.Y &&
                newPosition.Z == lastPosition.Position.Z)
            {
                return new ThrottledPosition()
                {
                    Position = newPosition,
                    HasUpdated = false
                };
            }
            else
            {
                return new ThrottledPosition()
                {
                    Position = newPosition,
                    HasUpdated = true
                };
            }
        }
    }
}
