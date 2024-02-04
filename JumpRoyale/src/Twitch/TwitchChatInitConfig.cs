namespace TwitchChat;

public class TwitchChatInitConfig(string jsonConfigPath, bool skipLocalConfig, bool automaticallyConnectToTwitch = true)
{
    public string JsonConfigPath { get; init; } = jsonConfigPath;

    public bool SkipLocalConfig { get; init; } = skipLocalConfig;

    public bool AutomaticallyConnectToTwitch { get; init; } = automaticallyConnectToTwitch;
}
