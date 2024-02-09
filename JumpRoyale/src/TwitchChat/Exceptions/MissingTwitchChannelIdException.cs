using System;
using TwitchChat.Messages;

namespace TwitchChat.Exceptions;

public class MissingTwitchChannelIdException : Exception
{
    public MissingTwitchChannelIdException()
        : base(TwitchMessages.ExceptionMissingTwitchChannelId) { }

    public MissingTwitchChannelIdException(string message)
        : base(message) { }

    public MissingTwitchChannelIdException(string message, Exception innerException)
        : base(message, innerException) { }
}
