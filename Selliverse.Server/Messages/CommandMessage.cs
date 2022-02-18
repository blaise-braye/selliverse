namespace Selliverse.Server.Messages
{
    public class CommandMessage
    {
        public string Id { get; set; }

        public string Type { get; set; } = "command";

        public string Content { get; set; }

    }
}
