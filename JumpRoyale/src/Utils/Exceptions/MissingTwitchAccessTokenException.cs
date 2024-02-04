using System;
using Constants.Messages;

namespace Utils.Exceptions;

public class MissingTwitchAccessTokenException : Exception
{
    public MissingTwitchAccessTokenException()
        : base(TwitchMessages.ExceptionMissingTwitchAccessToken) { }

    public MissingTwitchAccessTokenException(string message)
        : base(message) { }

    public MissingTwitchAccessTokenException(string message, Exception innerException)
        : base(message, innerException) { }
}
