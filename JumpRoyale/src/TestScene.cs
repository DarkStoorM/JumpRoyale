using Godot;
using TwitchChat;

namespace JumpRoyale;

public partial class TestScene : Node2D
{
    public override void _Ready()
    {
        TwitchChatInitConfig twitchInit = new(ResourcePaths.MainTwitchConfig, false);
        TwitchChatClient twitchChatClient = new(twitchInit);
    }
}
