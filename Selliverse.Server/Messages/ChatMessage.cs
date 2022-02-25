using System.Collections.Generic;

namespace Selliverse.Server.Messages
{
    public class ChatMessage : IMessage, IIncomingMessageParser
    {
        public string Id { get; set; }

        public string Type { get; set; } = "chat";

        public string Name { get; set; }

        public string Content { get; set; }

        public IMessage Parse(Dictionary<string, string> input) =>
            new ChatMessage()
            {
                Content = input["content"]
            };
    }
}
