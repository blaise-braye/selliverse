namespace Selliverse.Server.Messages
{
    public class PlayerEnteredGameMessage : IMessage
    {
        public string Type { get; set; } = "enter";

        public string Id { get; set; }

        public string Name { get; set; }
    }
}
