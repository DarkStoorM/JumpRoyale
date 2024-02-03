namespace TwitchChat;

public class TwitchChatInitConfig(string jsonConfigPath, bool skipLocalConfig)
{
    public string JsonConfigPath { get; init; } = jsonConfigPath;

    public bool SkipLocalConfig { get; init; } = skipLocalConfig;
}
