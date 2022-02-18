namespace Selliverse.Server.Messages
{
    public class PlayerEnteredGameMessage
    {
        public string Type { get; set; } = "entered";
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
