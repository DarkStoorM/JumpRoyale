using Godot;
using JumpRoyale.Commands;
using JumpRoyale.Constants;
using JumpRoyale.Events;
using JumpRoyale.Utils;
using JumpRoyale.Utils.Exceptions;
using TwitchChat;
using TwitchChat.Extensions;

namespace JumpRoyale;

public partial class TwitchManagerScene : Node2D
{
    [Export]
    public PackedScene? JumperScene { get; private set; }

    public override void _Ready()
    {
        CharacterSpriteProvider.Initialize();
        TwitchChatClient.Initialize(new(skipLocalConfig: false));
        PlayerStats.Initialize(ProjectSettings.GlobalizePath(ResourcePaths.StatsFilePath));

        TwitchChatClient.Instance.OnTwitchMessageReceivedEvent += OnMessageReceived;
        PlayerStats.Instance.OnPlayerJoin += OnPlayerJoin;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // NOTE: This simulates fake events with random data, this is not useful for actual tests on scene if there is
        // some data that has to align with Tests.

        // Ugly, fake command execution
        // Fake Join
        if (Input.IsPhysicalKeyPressed(Key.J))
        {
            FakeMessage("join");
        }
        else if (Input.IsPhysicalKeyPressed(Key.Q))
        {
            FakeMessage("r40");
        }
        else if (Input.IsPhysicalKeyPressed(Key.A))
        {
            FakeMessage("l10");
        }
        else if (Input.IsPhysicalKeyPressed(Key.S))
        {
            FakeMessage("l5");
        }
        else if (Input.IsPhysicalKeyPressed(Key.D))
        {
            FakeMessage("u");
        }
        else if (Input.IsPhysicalKeyPressed(Key.F))
        {
            FakeMessage("r5");
        }
        else if (Input.IsPhysicalKeyPressed(Key.G))
        {
            FakeMessage("r10");
        }
        else if (Input.IsPhysicalKeyPressed(Key.W))
        {
            FakeMessage("unglow");
        }
        else if (Input.IsPhysicalKeyPressed(Key.E))
        {
            FakeMessage("char 1");
        }
        else if (Input.IsPhysicalKeyPressed(Key.R))
        {
            FakeMessage("glow");
        }
        else if (Input.IsPhysicalKeyPressed(Key.T))
        {
            FakeMessage("glow random");
        }
        else if (Input.IsPhysicalKeyPressed(Key.Y))
        {
            FakeMessage("namecolor random");
        }
        else if (Input.IsPhysicalKeyPressed(Key.U))
        {
            FakeMessage($"char {Rng.IntRange(1, 18)}");
        }
    }

    /// <summary>
    /// Fakes the twitch event messages send by "admin" (just the same user), because we might want to test how things
    /// look without visiting Twitch just for that.
    /// </summary>
    /// <param name="message">Chat message to send to the fake event dispatcher.</param>
    /// <param name="isPrivileged">Allows overriding the privileges for this particular chat message.</param>
    private void FakeMessage(string message, bool isPrivileged = true)
    {
        TwitchChatClient.Instance.InvokeFakeMessageEvent(message, "DummyJumper", "666", "0fc", isPrivileged);
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

    /// <summary>
    /// Action automatically called when a new event is raised by the PlayerStats upon joining the game.
    /// </summary>
    private void OnPlayerJoin(object sender, PlayerJoinEventArgs eventArgs)
    {
        NullGuard.ThrowIfNull<UnassignedSceneOrComponentException>(JumperScene);

        JumperScene jumperScene = (JumperScene)JumperScene.Instantiate();

        AddChild(jumperScene);
        jumperScene.Init(eventArgs.Jumper);

        int x = 500;
        int y = 40;

        jumperScene.Position = new Vector2(x, y);
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
}
