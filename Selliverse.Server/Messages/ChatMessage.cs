namespace Selliverse.Server.Messages
{
    public class ChatMessage
    {
        public string Id { get; set; }

        public string Type { get; set; } = "chat";

        public string Name { get; set; }

        public string Content { get; set; }

    }
}
