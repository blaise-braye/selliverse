namespace Selliverse.Server.Messages
{
    public class RotationMessage
    {
        public string Type { get; set; } = "rotation";

        public string Id { get; set; }

        public float X { get; set; }

    }
}
