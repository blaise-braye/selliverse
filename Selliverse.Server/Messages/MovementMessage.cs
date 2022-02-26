using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Selliverse.Server.Messages
{
    public class MovementMessage : IMessage, IIncomingMessageParser
    {
        public string Id { get; set; }

        public string Type { get; set; } = "movement";
        
        public string X { get; set; }

        public string Y { get; set; }

        public string Z { get; set; }

        [JsonIgnore]
        public Vector3 Position => new System.Numerics.Vector3(
            float.Parse(X, CultureInfo.InvariantCulture),
            float.Parse(Y, CultureInfo.InvariantCulture),
            float.Parse(Z, CultureInfo.InvariantCulture));

        public IMessage Parse(Dictionary<string, string> input) =>
            new MovementMessage()
            {
                X=input["x"],
                Y = input["y"],
                Z = input["z"]
                
            };
    }
}
