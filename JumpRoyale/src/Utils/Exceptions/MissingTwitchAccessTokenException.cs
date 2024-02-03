using System;
using Constants.Twitch;

public class MissingTwitchAccessTokenException : Exception
{
    public MissingTwitchAccessTokenException()
        : base(
            $"No access token found. Please run `dotnet user-secrets set {TwitchConstants.ConfigAccessTokenIndex} <your access token>`"
        )
    { }

    public MissingTwitchAccessTokenException(string message)
        : base(message) { }

    public MissingTwitchAccessTokenException(string message, Exception innerException)
        : base(message, innerException) { }
}
