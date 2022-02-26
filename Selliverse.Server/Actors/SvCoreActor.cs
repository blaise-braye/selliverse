using Akka.Actor;
using Selliverse.Server.MessagesAsks;
using Selliverse.Server.Messages;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace Selliverse.Server.Actors
{
    public class SvCoreActor : ReceiveActor
    {
        public const int ChatDistance = 1000;

        private readonly Dictionary<string, Connection> _playerConnectionsById = new();
        
        private readonly Queue<ChatMessage> _lastMessages = new();

        public readonly IActorRef SvThrottledMovementActor;
        public readonly IActorRef SvThrottledRotationActor;

        public SvCoreActor()
        {
            SvThrottledMovementActor = Context.ActorOf(Props.Create(() => new SvThrottledMessageActor<MovementMessage>(Self)), "SvThrottledMovementActor");
            SvThrottledRotationActor = Context.ActorOf(Props.Create(() => new SvThrottledMessageActor<RotationMessage>(Self)), "SvThrottledRotationActor");

            this.Receive<PlayerConnectedMessage>(this.HandlePlayerConnected);
            this.ReceiveAsync<PlayerLeftMessage>(this.HandlePlayerLeft);
            this.ReceiveAsync<ChatMessage>(this.HandleChat);
            this.ReceiveAsync<CommandMessage>(this.HandleCommand);
            this.ReceiveAsync<PlayerEnteredGameMessage>(this.HandlePlayerEnteredGame);
            this.Receive<PlayerListAsk>(this.HandlePlayerListAsk);
            this.Receive<MovementMessage>(this.HandleMovement);
            this.Receive<RotationMessage>(this.HandleRotation);
            this.ReceiveAsync<BroadcastToOtherMessage<MovementMessage>>(this.HandleBroadcastToOthers);
            this.ReceiveAsync<BroadcastToOtherMessage<RotationMessage>>(this.HandleBroadcastToOthers);
        }
        
        private IEnumerable<Connection> Connections => _playerConnectionsById.Values;

        private Connection GetSender(IMessage msg)
        {
            return _playerConnectionsById[msg.Id];
        }

        private async Task BroadCastToOthers(IMessage message)
        {
            foreach (var cn in Connections.Where(cn => !cn.Id.Equals(message.Id, System.StringComparison.Ordinal)))
            {
                await cn.WebSocket.SendItRight(message);
            }
        }

        private async Task BroadcastChat(ChatMessage message, Connection sender)
        {
            var senderPos = sender.PlayerState.Position;
            foreach (var receiver in Connections)
            {
                // calculate distance
                var distance = Vector3.Distance(receiver.PlayerState.Position, senderPos);

                if (distance < ChatDistance)
                {
                    await receiver.WebSocket.SendItRight(message);
                }
            }
        }

        private async Task SendMoveCommand(PlayerState sender, string receiver)
        {
            var moveMessage = new MoveMessage
            {
                x = sender.Position.X.ToString(CultureInfo.InvariantCulture),
                y = (sender.Position.Y + 8f).ToString(CultureInfo.InvariantCulture),
                z = sender.Position.Z.ToString(CultureInfo.InvariantCulture)
            };

            if (this._playerConnectionsById.TryGetValue(receiver, out var connection))
            {
                await connection.WebSocket.SendItRight(moveMessage);
            }
        }

        private async Task HandleBroadcastToOthers<TMessage>(BroadcastToOtherMessage<TMessage> envelope) where TMessage : IMessage
        {
            await BroadCastToOthers(envelope.Message);
        }

        private void HandlePlayerConnected(PlayerConnectedMessage msg)
        {
            Log.Information("New player {id}", msg.Id);
            this._playerConnectionsById.Add(msg.Id, new Connection
            {
                Id = msg.Id,
                WebSocket = msg.WebSocket,
                PlayerState = new PlayerState
                {
                    GameState = GameState.Lobby
                }
            });
        }

        private async Task HandlePlayerLeft(PlayerLeftMessage msg)
        {
            Log.Information("Player {id} left", msg.Id);
            this._playerConnectionsById.Remove(msg.Id);
            await BroadCastToOthers(msg);
        }

        private async Task HandleCommand(CommandMessage msg)
        {
            Log.Information("{id}: {content}", msg.Id, msg.Content);
            // look up the name
            if (this._playerConnectionsById.TryGetValue(msg.Id, out var sender))
            {
                var message = msg.Content.Split(' ');
                if (message.Length == 2)
                {
                    if(message[0].Equals("meet", StringComparison.OrdinalIgnoreCase))
                    {
                        var target = this.Connections.FirstOrDefault(x => x.PlayerState.Name.Equals(message[1], StringComparison.OrdinalIgnoreCase));
                        if (target != null)
                        {
                            await SendMoveCommand(sender.PlayerState, target.Id);
                        }
                    }
                }
                Log.Information("Command received by {id}", msg.Id);
            }
        }

        private async Task HandleChat(ChatMessage msg)
        {
            Log.Information("{id}: {content}", msg.Id, msg.Content);
            // look up the name
            if (this._playerConnectionsById.TryGetValue(msg.Id, out var sender))
            {
                if (_lastMessages.Count > 4)
                {
                    _lastMessages.Dequeue();
                }

                var chatMessage = new ChatMessage
                {
                    Content = msg.Content,
                    Name = sender.PlayerState.Name,
                    Id = msg.Id
                };

                _lastMessages.Enqueue(chatMessage);

                await BroadcastChat(chatMessage, sender);
            }

        }

        private async Task HandlePlayerEnteredGame(PlayerEnteredGameMessage msg)
        {
            Log.Information("Getting a new player entered!");
            if (this.Connections.Any(cn => string.Equals(cn.PlayerState.Name, msg.Name, StringComparison.OrdinalIgnoreCase)))
            {
                // not allowed
                await GetSender(msg).SendItRight(new PlayerWelcomeMessage
                {
                    IsWelcome = false
                });
            }
            else
            {
                var sender = GetSender(msg);
                sender.PlayerState.Name = msg.Name;
                sender.PlayerState.GameState = GameState.InGame;
                await sender.SendItRight(new PlayerWelcomeMessage
                {
                    IsWelcome = true,
                });

                foreach (var message in _lastMessages)
                {
                    await sender.SendItRight(message);
                }
                
                foreach (var otherCn in Connections.Where(c => c != sender && c.PlayerState.GameState == GameState.InGame))
                {
                    var playerState = otherCn.PlayerState;
                    await otherCn.SendItRight(new PlayerEnteredGameMessage
                    {
                        Id = msg.Id,
                        Name = msg.Name

                    });
                    await sender.SendItRight(new PlayerEnteredGameMessage
                    {
                        Id = otherCn.Id,
                        Name = playerState.Name,
                    });
                }
            }
        }

        private void HandleRotation(RotationMessage msg)
        {
            GetSender(msg).PlayerState.RotationX = msg.XFloat;
            this.SvThrottledRotationActor.Tell(msg);
        }

        private void HandleMovement(MovementMessage msg)
        {
            GetSender(msg).PlayerState.Position = msg.Position;
            this.SvThrottledMovementActor.Tell(msg);
        }

        private void HandlePlayerListAsk(PlayerListAsk ask)
        {
            var response = Connections.Select(ps => new ConnectedPlayer
            {
                Id = ps.Id,
                Name = ps.PlayerState.Name
            }).ToArray();

            Sender.Tell(new PlayerListResponse { Players = response });
        }
    }
}
