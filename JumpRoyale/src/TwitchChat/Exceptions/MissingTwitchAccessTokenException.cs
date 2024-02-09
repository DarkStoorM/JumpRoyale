using System;
using TwitchChat.Messages;

namespace TwitchChat.Exceptions;

public class MissingTwitchAccessTokenException : Exception
{
    public MissingTwitchAccessTokenException()
        : base(TwitchMessages.ExceptionMissingTwitchAccessToken) { }

    public MissingTwitchAccessTokenException(string message)
        : base(message) { }

    public MissingTwitchAccessTokenException(string message, Exception innerException)
        : base(message, innerException) { }
}
