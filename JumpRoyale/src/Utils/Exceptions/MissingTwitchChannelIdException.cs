using System;
using Constants.Messages;

namespace Utils.Exceptions;

public class MissingTwitchChannelIdException : Exception
{
    public MissingTwitchChannelIdException()
        : base(TwitchMessages.ExceptionMissingTwitchChannelId) { }

    public MissingTwitchChannelIdException(string message)
        : base(message) { }

    public MissingTwitchChannelIdException(string message, Exception innerException)
        : base(message, innerException) { }
}
