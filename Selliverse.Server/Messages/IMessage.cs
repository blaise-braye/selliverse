namespace Selliverse.Server.Messages;

public interface IMessage
{
    /// <summary>
    /// Id of the sender
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Type of the message
    /// </summary>
    string Type { get; }
}