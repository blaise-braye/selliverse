namespace Selliverse.Server.Messages
{
    public class CommandMessage : IMessage
    {
        public string Id { get; set; }

        public string Type { get; set; } = "command";

        public string Content { get; set; }

    }
}
