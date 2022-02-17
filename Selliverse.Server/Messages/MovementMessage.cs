using System.Numerics;

namespace Selliverse.Server.Messages
{
    public class MovementMessage
    {
        public string Id { get; set; }

        public Vector3 Position { get; set; }
    }
}
