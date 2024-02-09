using TwitchChat.Constants;

namespace TwitchChat;

/// <summary>
/// Twitch initialization config object containing paths to the Json files with channel name/id.
/// <para>
/// Channel Name is used as part of Twitch Credentials along with the Access Token.
/// </para>
/// <para>
/// Channel Id is used to specify where to listen for the events from.
/// </para>
/// </summary>
/// <remarks>
/// This class can be instantiated with no arguments, meaning that the defaults are used as defined on the arguments
/// list in the provided constructor.
/// </remarks>
public class TwitchChatInitConfig(
    string? jsonConfigPath = null,
    string? localConfigFile = null,
    bool skipLocalConfig = true,
    bool automaticallyConnectToTwitch = true
)
{
    /// <summary>
    /// Path to the main Json config file that contains the channel name/id it should connect to/read from. Override if
    /// the config has to be read from different path than default. Default path: <see
    /// cref="TwitchConfigPaths.MainTwitchConfig"/>.
    /// </summary>
    public string JsonConfigPath { get; } = jsonConfigPath ?? TwitchConfigPaths.MainTwitchConfig;

    /// <summary>
    /// Local (untracked) Json configuration file for overriding purposes. If this file exists and was requested during
    /// the Twitch Chat Client initialization, it will override the data loaded by the main Json file. By default, path
    /// to this config is <c>./Config/Local.json</c>.
    /// </summary>
    /// <remarks>
    /// This file may not exist, because it is not required by the codebase. It's only used to override the keep the
    /// main config intact and develop on a Local config with different data that does not have to be committed to the
    /// Version Control.
    /// </remarks>
    public string LocalConfigPath { get; } = localConfigFile ?? TwitchConfigPaths.LocalTwitchConfig;

    /// <summary>
    /// If <c>true</c>, omits the additional Json config load during the Twitch Chat initialization. Local configs are
    /// useful when developing on a different Twitch channel.
    /// </summary>
    public bool SkipLocalConfig { get; } = skipLocalConfig;

    /// <summary>
    /// Flag to instruct the TwitchChatClient to omit connection to the Twitch services. This flag is for testing
    /// features that are not related to twitch, e.g. running Tests quicker or in situations where the connection might
    /// not be available at the moment to not force the client to waste time on trying to connect.
    /// </summary>
    /// <remarks>
    /// This will only omit the Twitch service connection, leaving the event listeners open, since we still want to
    /// manually invoke the internal Twitch events for testing purposes.
    /// </remarks>
    public bool AutomaticallyConnectToTwitch { get; } = automaticallyConnectToTwitch;
}
