﻿using Selliverse.Server.MessagesAsks;

namespace Selliverse.Server.Actors
{
    using Akka.Actor;
    using Selliverse.Server.Messages;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.WebSockets;
    using System.Numerics;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    public class SvCoreActor : ReceiveActor
    {
        public const int CHAT_DISTANCE = 10;

        private Dictionary<string, WebSocket> playerConnections = new Dictionary<string, WebSocket>();

        private Dictionary<string, PlayerState> playerStates = new Dictionary<string, PlayerState>();

        private Queue<ChatMessage> lastMessages = new Queue<ChatMessage>();

        public readonly IActorRef throttleActor;

        public SvCoreActor(IActorRef throttleActor)
        {
            this.Receive<PlayerConnectedMessage>(this.HandlePlayerConnected);
            this.Receive<PlayerLeftMessage>(this.HandlePlayerLeft);
            this.ReceiveAsync<ChatMessage>(this.HandleChat);
            this.ReceiveAsync<PlayerEnteredGameMessage>(this.HandlePlayerEnteredGame);
            this.Receive<PlayerListAsk>(this.HandlePlayerListAsk);
            this.Receive<MovementMessage>(this.HandleMovement);
            this.throttleActor = throttleActor;
        }


        private async Task BroadCastToOthers(string id, object message)
        {
            foreach (var (_, socket) in playerConnections.Where(kvp => !kvp.Key.Equals(id, System.StringComparison.Ordinal)))
            {
                await socket.SendItRight(message);
            }
        }

        private async Task BroadCastToAll(string id, object message)
        {
            foreach (var (_, socket) in playerConnections)
            {
                await socket.SendItRight(message);
            }
        }

        private async Task BroadcastChat(ChatMessage message, PlayerState sender)
        {
            var senderPos = sender.Position;
            foreach (var player in playerStates.Where(pc => !string.Equals(sender.Name, pc.Value.Name, StringComparison.OrdinalIgnoreCase)))
            {
                // calculate distance
                var distance = Vector3.Distance(player.Value.Position, senderPos);

                if (distance < CHAT_DISTANCE)
                {
                    if (this.playerConnections.TryGetValue(player.Key, out var receiver))
                    {
                        await receiver.SendItRight(message);
                    }
                }
            }
        }

        private void HandlePlayerConnected(PlayerConnectedMessage msg)
        {
            Log.Information("New player {id}", msg.Id);
            this.playerConnections.Add(msg.Id, msg.WebSocket);
            this.playerStates.Add(msg.Id, new PlayerState()
            {
                GameState = GameState.Lobby
            });
        }

        private void HandlePlayerLeft(PlayerLeftMessage msg)
        {
            Log.Information("Player {id} left", msg.Id);
            this.playerConnections.Remove(msg.Id);
            this.playerStates.Remove(msg.Id);
        }

        private async Task HandleChat(ChatMessage msg)
        {
            Log.Information("{id}: {content}", msg.Id, msg.Content);
            // look up the name
            if (this.playerStates.TryGetValue(msg.Id, out var sender))
            {
                if (lastMessages.Count > 4)
                {
                    lastMessages.Dequeue();
                }

                var chatMessage = new ChatMessage()
                {
                    Content = msg.Content,
                    Name = sender.Name,
                    Id = msg.Id
                };

                lastMessages.Enqueue(chatMessage);

                await BroadcastChat(chatMessage, sender);
            }

        }

        private async Task HandlePlayerEnteredGame(PlayerEnteredGameMessage msg)
        {
            Log.Information("Getting a new player entered!");
            if (this.playerStates.Values.Any(ps => String.Equals(ps.Name, msg.Name, StringComparison.OrdinalIgnoreCase)))
            {
                // not allowed
                await this.playerConnections[msg.Id].SendItRight(new PlayerWelcomeMessage()
                {
                    IsWelcome = false
                });
            }
            else
            {
                this.playerStates[msg.Id].Name = msg.Name;
                this.playerStates[msg.Id].GameState = GameState.InGame;
                await this.playerConnections[msg.Id].SendItRight(new PlayerWelcomeMessage()
                {
                    IsWelcome = true,
                });

                foreach (var message in lastMessages)
                {
                    await this.playerConnections[msg.Id].SendItRight(message);
                }

                //foreach(var player in this.playerConnections.Where(pc => !string.Equals(pc.Key, msg.Id, StringComparison.OrdinalIgnoreCase)))
                //{
                //    await player.Value.SendItRight(new ChatMessage()
                //    {
                //        Content = $"Player {msg.Name} has entered the Selliverse",
                //        Name = msg.Name
                //    });
                //}

                foreach (var (id, player) in this.playerConnections.Where(pc => !string.Equals(pc.Key, msg.Id, StringComparison.OrdinalIgnoreCase)))
                {
                    if (this.playerStates.TryGetValue(id, out PlayerState otherPlayer))
                    {
                        if (otherPlayer.GameState == GameState.InGame)
                        {
                            await player.SendItRight(new PlayerEnteredGameMessage()
                            {
                                Id = id,
                                Name = otherPlayer.Name,
                                Type = "entered"
                            });
                        }
                    }
                }


            }
        }

        private void HandleMovement(MovementMessage msg)
        {
            this.throttleActor.Tell(msg);
        }

        private void HandlePlayerListAsk(PlayerListAsk ask)
        {
            var response = playerStates.Select(ps =>
            {
                return new ConnectedPlayer()
                {
                    Id = ps.Key,
                    Name = ps.Value.Name
                };
            }).ToArray();

            Sender.Tell(new PlayerListResponse() { Players = response });
        }
    }
}
