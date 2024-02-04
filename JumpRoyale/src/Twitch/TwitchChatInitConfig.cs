namespace TwitchChat;

public class TwitchChatInitConfig(string jsonConfigPath, bool skipLocalConfig, bool automaticallyConnectToTwitch = true)
{
    /// <summary>
    /// Path to the main Json config file that contains the channel name/id it should connect to/read from. Override if
    /// the config has to be read from different path than default.
    /// </summary>
    public string JsonConfigPath { get; init; } = jsonConfigPath;

    /// <summary>
    /// If <c>true</c>, omits the additional Json config load during the Twitch Chat initialization. Local configs are
    /// useful when developing on a different Twitch channel. By default, path to this config is
    /// <c>./Config/Local.json</c>.
    /// </summary>
    /// <remarks>
    /// This file may not exist, because it is not required by the codebase. It's only used to override the keep the
    /// main config intact and develop on a Local config with different data that does not have to be committed to the
    /// Version Control.
    /// </remarks>
    public bool SkipLocalConfig { get; init; } = skipLocalConfig;

    /// <summary>
    /// Flag to instruct the TwitchChatClient to omit connection to the Twitch services. This flag is for testing
    /// features that are not related to twitch, e.g. running Tests quicker or in situations where the connection might
    /// not be available at the moment to not force the client to waste time on trying to connect.
    /// </summary>
    /// <remarks>
    /// This will only omit the Twitch service connection, leaving the event listeners open, since we still want to
    /// manually invoke the internal Twitch events for testing purposes.
    /// </remarks>
    public bool AutomaticallyConnectToTwitch { get; init; } = automaticallyConnectToTwitch;
}
