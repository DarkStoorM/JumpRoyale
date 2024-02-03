using System;
using Constants.Twitch;

public class MissingTwitchChannelNameException : Exception
{
    public MissingTwitchChannelNameException()
        : base(
            $"Channel Name not found. Please add your Twitch channel name into the {TwitchConstants.ConfigChannelNameIndex} key in TwitchConfig.json configuration file.`"
        )
    { }

    public MissingTwitchChannelNameException(string message)
        : base(message) { }

    public MissingTwitchChannelNameException(string message, Exception innerException)
        : base(message, innerException) { }
}
