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
        private readonly IActorRef coreActor;

        public SocketTranslator(ActorSystem actorSystem)
        {
            var props = Props.Create<SvCoreActor>();
            this.coreActor = actorSystem.ActorOf(props, "svCore");
        }

        public void OnConnected(string id, WebSocket webSocket)
        {
            Log.Information("SV new player {id}", id);

            this.coreActor.Tell(new PlayerJoinedMessage()
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
                this.coreActor.Tell(msg);
            }
        }

        public void OnDisconnected(string id)
        {
            Log.Information("SV lost player {id}", id);
            this.coreActor.Tell(new PlayerLeftMessage()
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
                case "name":
                    return new PlayerNameSetMessage()
                    {
                        Id = id,
                        Name = input["name"]
                    };
                default:
                    return null;
            }
        }
    }
}
