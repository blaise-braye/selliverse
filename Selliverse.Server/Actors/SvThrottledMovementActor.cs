using Akka.Actor;
using Selliverse.Server.Messages;
using System;
using System.Collections.Generic;

namespace Selliverse.Server.Actors
{
    public class SvThrottledMessageActor<TMessage> : ReceiveActor where TMessage : IMessage
    {
        private readonly Dictionary<string, TMessage> _latestSendersMessage = new();

        private readonly IActorRef _daddy;

        public SvThrottledMessageActor(IActorRef daddyActor)
        {
            this._daddy = daddyActor;
            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.Zero, TimeSpan.FromMilliseconds(50), Self, new SendThrottledMessage<TMessage>(), Self);
            this.Receive<TMessage>(this.HandleMovement);
            this.Receive<SendThrottledMessage<TMessage>>(this.HandleSendThrottledMessage);
        }

        private void HandleSendThrottledMessage(SendThrottledMessage<TMessage> @event)
        {
            foreach (var movement in this._latestSendersMessage.Values)
            {
                this._daddy.Tell(BroadcastToOtherMessage.Create(movement));
            }

            this._latestSendersMessage.Clear();
        }

        private void HandleMovement(TMessage msg)
        {
            this._latestSendersMessage[msg.Id] = msg;
        }
    }
}
