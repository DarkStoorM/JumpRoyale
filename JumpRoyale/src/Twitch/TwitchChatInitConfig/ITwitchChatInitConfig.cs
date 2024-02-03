namespace TwitchChat;

public interface ITwitchChatInitConfig
{
    string JsonConfigPath { get; init; }

    bool SkipLocalConfig { get; init; }
}
