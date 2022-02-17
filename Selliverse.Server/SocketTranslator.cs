using Akka.Actor;
using Selliverse.Server.Actors;
using Selliverse.Server.Messages;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text.Json;

namespace Selliverse.Server
{
    public class SocketTranslator
    {
        private IActorRef CoreActor { get; }

        public SocketTranslator(IActorRef coreActor)
        {
            CoreActor = coreActor;
        }

        public void OnConnected(string id, WebSocket webSocket)
        {
            Log.Information("SV new player {id}", id);

            this.CoreActor.Tell(new PlayerConnectedMessage()
            {
                Id = id,
                WebSocket = webSocket
            });
        }

        public void OnMessage(string id, string message)
        {
            var payload = JsonSerializer.Deserialize<Dictionary<string, string>>(message);
            // Log.Warning("MSG OF TYPE {type}", payload["type"]);
            // ChatMessage cm = dynob as ChatMessage;

            var msg = CreateStronglyTypedMessage(payload, id);
            if (msg != null)
            {
                this.CoreActor.Tell(msg);
            }
        }

        public void OnDisconnected(string id)
        {
            Log.Information("SV lost player {id}", id);
            this.CoreActor.Tell(new PlayerLeftMessage()
            {
                Id = id
            });
        }

        private Object CreateStronglyTypedMessage(Dictionary<string, string> input, String id)
        {
            switch (input["type"])
            {
                case "chat":
                    return new ChatMessage()
                    {
                        Id = id,
                        Content = input["content"]
                    };
                case "enter":
                    return new PlayerEnteredGameMessage()
                    {
                        Id = id,
                        Name = input["name"]
                    };
                case "movement":
                    return new MovementMessage()
                    {
                        Id = id,
                        Position = new System.Numerics.Vector3(float.Parse(input["x"]), float.Parse(input["y"]), float.Parse(input["z"]))
                    };
                default:
                    return null;
            }
        }
    }
}
