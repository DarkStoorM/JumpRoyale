namespace TwitchChat.Constants;

public static class TwitchConfigPaths
{
    public const string MainTwitchConfig = "\\Config\\Twitch.json";

    /// <summary>
    /// Path to the optional config file with same settings as <c>MainTwitchConfig</c>. Loading this config will replace
    /// overlapping keys from the Main config or user-secrets, if any.
    /// </summary>
    public const string LocalTwitchConfig = "\\Config\\Local.json";
}
