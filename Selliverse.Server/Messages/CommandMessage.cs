using System.Collections.Generic;

namespace Selliverse.Server.Messages
{
    public class CommandMessage : IMessage, IIncomingMessageParser
    {
        public string Id { get; set; }

        public string Type { get; set; } = "command";

        public string Content { get; set; }

        public IMessage Parse(Dictionary<string, string> input) =>
            new CommandMessage
            {
                Content = input["content"],
            };
    }
}
