using System.Collections.Generic;
using System.Globalization;
using System.Text.Json.Serialization;

namespace Selliverse.Server.Messages
{
    public class RotationMessage : IMessage, IIncomingMessageParser
    {
        public string Type { get; set; } = "rotation";

        public string Id { get; set; }

        public string X { get; set; }

        [JsonIgnore]
        public float XFloat => float.Parse(X, CultureInfo.InvariantCulture);

        public IMessage Parse(Dictionary<string, string> input)
        {
            return new RotationMessage()
            {
                X = input["x"]
            };
        }
    }
}
