namespace Selliverse.Server.Messages
{
    public class PlayerLeftMessage : IMessage
    {
        public string Type { get; set; } = "left";

        public string Id { get; set; }
    }
}
