namespace Constants.Twitch;

public static class TwitchConstants
{
    /// <summary>
    /// User-secrets Access Token index for IConfigurationRoot.
    /// </summary>
    public const string ConfigAccessTokenIndex = "twitch_access_token";

    /// <summary>
    /// Json Channel Name index for IConfigurationRoot.
    /// </summary>
    public const string ConfigChannelNameIndex = "twitch_channel_name";

    /// <summary>
    /// Json Channel Id index for IConfigurationRoot.
    /// </summary>
    public const string ConfigChannelIdIndex = "twitch_channel_id";

    /// <summary>
    /// Twitch ClientOptions constant for: <c>MessagesAllowedInPeriod</c>.
    /// </summary>
    public const int MaximumMessages = 750;

    /// <summary>
    /// Twitch ClientOptions constant for: <c>ThrottlingPeriod</c> TimeSpan in seconds.
    /// </summary>
    public const int ThrottlingInSeconds = 30;
}
