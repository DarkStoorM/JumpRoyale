using TwitchChat;

public class TwitchChatInitConfig(string jsonConfigPath, bool skipLocalConfig) : ITwitchChatInitConfig
{
    public string JsonConfigPath { get; init; } = jsonConfigPath;

    public bool SkipLocalConfig { get; init; } = skipLocalConfig;
}
