using System;
using Constants.Twitch;

public class MissingTwitchChannelIdException : Exception
{
    public MissingTwitchChannelIdException()
        : base(
            $"Channel Id not found. Please add your Twitch channel id into the {TwitchConstants.ConfigChannelIdIndex} key.`"
        )
    { }

    public MissingTwitchChannelIdException(string message)
        : base(message) { }

    public MissingTwitchChannelIdException(string message, Exception innerException)
        : base(message, innerException) { }
}
