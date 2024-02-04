using System;
using Constants.Messages;

namespace Utils.Exceptions;

public class MissingTwitchChannelNameException : Exception
{
    public MissingTwitchChannelNameException()
        : base(TwitchMessages.ExceptionMissingTwitchChannelName) { }

    public MissingTwitchChannelNameException(string message)
        : base(message) { }

    public MissingTwitchChannelNameException(string message, Exception innerException)
        : base(message, innerException) { }
}
