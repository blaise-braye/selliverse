namespace Selliverse.Server.Messages
{
    public class RotationMessage : IMessage
    {
        public string Type { get; set; } = "rotation";

        public string Id { get; set; }

        public float X { get; set; }

    }
}
