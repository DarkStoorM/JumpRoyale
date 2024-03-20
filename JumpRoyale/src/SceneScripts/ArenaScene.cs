using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <summary>
    /// Stores a dictionary of "Levels", where each level represents the range of platform lengths for that level.
    /// <para>
    /// "Level" changes every [x] platforms.
    /// </para>
    /// </summary>
    private readonly Dictionary<int, int[]> _platformLengths = Enumerable
        .Range(0, 11)
        .ToDictionary(i => i, i => new int[] { i, i + 6 });

    private ArenaBuilder _builder = null!;

    private Rect2 _viewport;

    private float _chanceToGeneratePlatform = 0.0075f;

    /// <summary>
    /// Note: Y up goes negative, hence the sign.
    /// </summary>
    private int _maximumArenaHeightInTiles = -600;

    /// <summary>
    /// Maximum allowed number of platforms to generate in a single row before forcing to go to the next row.
    /// </summary>
    private int _maximumPlatformsPerRow = 3;

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
        _builder = new ArenaBuilder(tileMap, 160);

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

    /// <summary>
    /// Pre-generates the entire arena, column-by-column, row-by-row, including extra blocks and different platform/wall
    /// shapes.
    /// </summary>
    private void GenerateArena()
    {
        DrawSideWalls();

        // Start drawing from the bottom, excluding the main floor up to the set maximum
        int startY = ViewportSizeInTiles.Y - _platformDrawingOffsetInTiles;
        int endY = _maximumArenaHeightInTiles;

        // Randomly draw the platforms as base grounding
        for (int y = startY; y > endY; y--)
        {
            int platformLength = GetPlatformLength(y);

            // Draw on the playable arena field, excluding the current platform length to prevent from drawing off
            // screen.
            int startingColumn = (int)ArenaRectInTiles.Position.X;
            int endingColumn = (int)ArenaRectInTiles.End.X - platformLength - 3;

            int platformsGeneratedThisRow = 0;
            int currentColumn = startingColumn - 1;

            while (currentColumn < endingColumn)
            {
                currentColumn++;

                if (Rng.RandomFloat() > _chanceToGeneratePlatform)
                {
                    continue;
                }

                _builder.DrawHorizontalPlatform(new(currentColumn, y), platformLength);
                platformsGeneratedThisRow++;

                // Skip as many columns as it took to draw the platform + offset (1 space + platform edges: L/R)
                currentColumn += platformLength;

                // Force skipping to the next row if we already generated enough platforms
                if (platformsGeneratedThisRow == _maximumPlatformsPerRow)
                {
                    break;
                }
            }
        }
    }

    private int GetPlatformLength(int currentY)
    {
        // NOTE: make a max/level step
        int index = Math.Abs(_maximumArenaHeightInTiles / currentY);
        int[] lengths = _platformLengths[index];
        int platformLength = Rng.IntRange(lengths[0], lengths[1]);

        return platformLength;
    }

    private void DrawSideWalls()
    {
        // The very bottom of the stage (-1 to still draw on-screen and -3, since the floor is 3 tiles tall)
        int startingYPoint = ViewportSizeInTiles.Y - 4;

        // Wall height to draw per "step", excluding two tiles for both edges, they are added automatically
        int wallHeightPerStep = 10; // temporary until some calculation is in place

        // We need to manually define the sprite differences on given height, since we are not using the same sprite to
        // draw everything, only the generated platform will change automatically.
        for (int i = 0; i < 3; i++)
        {
            // Left wall
            _builder.DrawVerticalWall(new(0, startingYPoint - (wallHeightPerStep * i)), wallHeightPerStep - 2);

            // Right wall
            // Note: Viewport is 120 tiles long, but we have to draw on the last column, otherwise we end up off screen
            _builder.DrawVerticalWall(
                new(ViewportSizeInTiles.X - 1, startingYPoint - (wallHeightPerStep * i)),
                wallHeightPerStep - 2
            );
        }
    }
}
