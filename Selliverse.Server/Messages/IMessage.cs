using System.Collections.Generic;

namespace Selliverse.Server.Messages;


public interface IIncomingMessageParser
{
    string Type { get; }

    IMessage Parse(Dictionary<string, string> input);
}

public interface IMessage
{
    /// <summary>
    /// Id of the sender
    /// </summary>
    string Id { get; set; }

    /// <summary>
    /// Type of the message
    /// </summary>
    string Type { get; }
}