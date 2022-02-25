using System.Collections.Generic;
using System.Globalization;

namespace Selliverse.Server.Messages
{
    public class RotationMessage : IMessage, IIncomingMessageParser
    {
        public string Type { get; set; } = "rotation";

        public string Id { get; set; }

        public float X { get; set; }

        public IMessage Parse(Dictionary<string, string> input)
        {
            return new RotationMessage()
            {
                X = float.Parse(input["x"], CultureInfo.InvariantCulture)
            };
        }
    }
}
