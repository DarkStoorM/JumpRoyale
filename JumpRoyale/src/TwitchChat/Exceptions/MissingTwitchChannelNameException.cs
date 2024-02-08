using System;
using TwitchChat.Messages;

namespace TwitchChat.Exceptions;

public class MissingTwitchChannelNameException : Exception
{
    public MissingTwitchChannelNameException()
        : base(TwitchMessages.ExceptionMissingTwitchChannelName) { }

    public MissingTwitchChannelNameException(string message)
        : base(message) { }

    public MissingTwitchChannelNameException(string message, Exception innerException)
        : base(message, innerException) { }
}
