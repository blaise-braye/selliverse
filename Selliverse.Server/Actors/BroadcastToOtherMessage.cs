using Selliverse.Server.Messages;

namespace Selliverse.Server.Actors;

public class BroadcastToOtherMessage
{
    public static BroadcastToOtherMessage<TMessage> Create<TMessage>(TMessage message) where TMessage : IMessage =>
        new BroadcastToOtherMessage<TMessage>()
        {
            Message = message
        };
}

public class BroadcastToOtherMessage<TMessage> where TMessage : IMessage
{
    public TMessage Message { get; set; }
}