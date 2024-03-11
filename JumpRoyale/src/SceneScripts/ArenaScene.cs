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
    private ArenaBuilder _builder = null!;

    private Rect2 _viewport;

    private int _maximumArenaHeight = -600;

    /// <summary>
    /// How many tiles to skip from the bottom of the screen before we start generating platforms.
    /// </summary>
    private int _platformDrawingOffsetInTiles = 5;

    /// <summary>
    /// Describes the "playable" arena field, which excludes the side walls (1 tile each) and is offset by 1 tile on
    /// x-axis on both sides - reducing the usable field by 4 tiles.
    /// </summary>
    /// <remarks>
    /// This property is constructed from values provided by <see cref="ViewportSizeInTiles"/>, where the Position
    /// (including End property) and Size have been converted from pixels to tiles.
    /// </remarks>
    public Rect2 ArenaRectInTiles =>
        new()
        {
            Position = new(2, 0),
            Size = new(ViewportSizeInTiles.X - 4, ViewportSizeInTiles.Y),
            End = new(ViewportSizeInTiles.X - 2, ViewportSizeInTiles.Y),
        };

    [Export]
    public PackedScene? JumperScene { get; private set; }

    /// <summary>
    /// Unscaled viewport size.
    /// </summary>
    public Vector2 ViewportSizeInPixels => _viewport.Size;

    /// <summary>
    /// Describes the visible TileMap portion of the viewport in tiles.
    /// </summary>
    public Vector2I ViewportSizeInTiles => new((int)_viewport.Size.X / 16, (int)_viewport.Size.Y / 16);

    public override void _Ready()
    {
        TileMap tileMap = GetNode<TileMap>("TileMap");

        if (tileMap.TileSet is null)
        {
            throw new UnassignedSceneOrComponentException();
        }

        _viewport = GetViewportRect();
        _builder = new ArenaBuilder(tileMap);

        TwitchChatClient.Initialize(new(skipLocalConfig: false));
        PlayerStats.Initialize(ProjectSettings.GlobalizePath(ResourcePaths.StatsFilePath));

        TwitchChatClient.Instance.OnTwitchMessageReceivedEvent += OnMessageReceived;
        PlayerStats.Instance.OnPlayerJoin += OnPlayerJoin;

        GenerateArena();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // NOTE: This simulates fake events with random data, this is not useful for actual tests on scene if there is
        // some data that has to align with Tests.

        // Fake Join
        if (Input.IsPhysicalKeyPressed(Key.J))
        {
            FakeMessage("join");
        }

        // Ugly, fake command execution
        if (Input.IsPhysicalKeyPressed(Key.Q))
        {
            FakeMessage("r40");
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
        int x = 500;
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

    private void GenerateArena()
    {
        DrawSideWalls();

        // Start drawing from the bottom, excluding the main floor up to the set maximum
        int startY = ViewportSizeInTiles.Y - _platformDrawingOffsetInTiles;
        int endY = _maximumArenaHeight;

        // Draw on the playable arena field, excluding the current platform length to prevent from drawing off screen.
        int startX = (int)ArenaRectInTiles.Position.X;
        int endX = (int)ArenaRectInTiles.End.X - 8; // <- note: 8 is temporary, this becomes PlatformLength later

        // Randomly draw the platforms as base grounding
        for (int y = startY; y > endY; y--)
        {
            for (int x = startX; x < endX; x++)
            {
                // Insert a platform here and maybe some logic, like changing the sprites
            }
        }
    }

    private void DrawSideWalls()
    {
        _builder.DrawVerticalWall(new(0, 63), 1000);
        _builder.DrawVerticalWall(new(119, 63), 1000);
    }
}
