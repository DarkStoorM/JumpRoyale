using TwitchChat;

namespace JumpRoyale;

public class CommandHandler(ChatMessageEventArgs args)
{
    public ChatMessageEventArgs ChatMessageEventArgs { get; } = args;
}
