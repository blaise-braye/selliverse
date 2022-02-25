using System.Collections.Generic;

namespace Selliverse.Server.Messages
{
    public class PlayerEnteredGameMessage : IMessage, IIncomingMessageParser
    {
        public string Type { get; set; } = "enter";

        public string Id { get; set; }

        public string Name { get; set; }

        public IMessage Parse(Dictionary<string, string> input) =>
            new PlayerEnteredGameMessage
            {
                Name = input["name"]
            };
    }
}
