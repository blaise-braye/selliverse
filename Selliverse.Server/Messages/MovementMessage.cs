using System.Collections.Generic;
using System.Globalization;
using System.Numerics;

namespace Selliverse.Server.Messages
{
    public class MovementMessage : IMessage, IIncomingMessageParser
    {
        public string Id { get; set; }

        public string Type { get; set; } = "movement";

        public Vector3 Position { get; set; }

        public IMessage Parse(Dictionary<string, string> input)
        {
            return new MovementMessage()
            {
                Position = new System.Numerics.Vector3(
                    float.Parse(input["x"], CultureInfo.InvariantCulture), 
                    float.Parse(input["y"], CultureInfo.InvariantCulture), 
                    float.Parse(input["z"], CultureInfo.InvariantCulture))
            };
        }
    }
}
