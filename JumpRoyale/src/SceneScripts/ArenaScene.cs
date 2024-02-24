using Godot;
using JumpRoyale.Commands;
using JumpRoyale.Constants;
using JumpRoyale.Events;
using JumpRoyale.Utils;
using JumpRoyale.Utils.Exceptions;
using TwitchChat;
using TwitchChat.Extensions;

namespace JumpRoyale;

public partial class ArenaScene : Node2D
{
    [Export]
    public PackedScene? JumperScene { get; private set; }

    public override void _Ready()
    {
        TwitchChatClient.Initialize(new());
        PlayerStats.Initialize(ProjectSettings.GlobalizePath(ResourcePaths.StatsFilePath));

        TwitchChatClient.Instance.OnTwitchMessageReceivedEvent += OnMessageReceived;
        PlayerStats.Instance.OnPlayerJoin += OnPlayerJoin;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Input.IsPhysicalKeyPressed(Key.J))
        {
            TwitchChatClient.Instance.InvokeFakeMessageEvent("join");
        }
    }

    /// <summary>
    /// Action automatically called when a new event is raised by Twitch Chat Client upon receiving a chat message.
    /// </summary>
    private void OnMessageReceived(object sender, ChatMessageEventArgs args)
    {
        CallDeferred(
            nameof(HandleChatMessage),
            args.Message,
            args.UserId,
            args.DisplayName,
            args.ColorHex,
            args.IsPrivileged
        );
    }

    /// <summary>
    /// Action automatically called when a new event is raised by the PlayerStats upon joining the game.
    /// </summary>
    private void OnPlayerJoin(object sender, PlayerJoinEventArgs eventArgs)
    {
        NullGuard.ThrowIfNull<UnassignedSceneOrComponentException>(JumperScene);

        JumperScene jumperScene = (JumperScene)JumperScene.Instantiate();

        // Rect2 viewport = GetViewportRect();
        // GD.Print(viewport);
        int x = 0;
        int y = 40;

        jumperScene.Init(eventArgs.Jumper);
        jumperScene.Position = new Vector2(x, y);

        AddChild(jumperScene);
    }

    /// <summary>
    /// Executes internal command if the provided chat message matches any of the aliases for those commands.
    /// </summary>
    private void HandleChatMessage(
        string message,
        string senderId,
        string senderName,
        string hexColor,
        bool isPrivileged
    )
    {
        ChatCommandHandler commandHandler = new(message, senderId, senderName, hexColor, isPrivileged);

        commandHandler.ProcessMessage();
    }
}
