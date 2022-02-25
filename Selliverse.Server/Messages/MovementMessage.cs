using System.Numerics;

namespace Selliverse.Server.Messages
{
    public class MovementMessage : IMessage
    {
        public string Id { get; set; }

        public string Type { get; set; } = "movement";

        public Vector3 Position { get; set; }
    }
}
