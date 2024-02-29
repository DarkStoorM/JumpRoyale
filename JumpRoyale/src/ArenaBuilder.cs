using System;
using System.Collections.Generic;
using Godot;
using JumpRoyale.Utils;

namespace JumpRoyale;

public class ArenaBuilder
{
    private static readonly object _lock = new();
    private static ArenaBuilder? _instance;

    private readonly TileSet _tileSet;

    /// <summary>
    /// Collection of all Platforms that can be used for drawing one-way collision platforms.
    /// </summary>
    private readonly Dictionary<GameTileTypes, BasePlatform> _platforms = [];

    private ArenaBuilder(TileSet tileSet)
    {
        _tileSet = tileSet;

        _platforms.Add(GameTileTypes.PlatformConcrete, GameTiles.PlatformConcrete);
        _platforms.Add(GameTileTypes.PlatformGold, GameTiles.PlatformGold);
        _platforms.Add(GameTileTypes.PlatformStone, GameTiles.PlatformStone);
    }

    public static ArenaBuilder Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance is null)
                {
                    throw new InvalidOperationException("Call Initialize() before using ArenaBuilder.");
                }

                return _instance;
            }
        }
    }

    /// <summary>
    /// Creates ArenaBuilder instance with TileSet containing sprites to assign to platforms and other objects (walls,
    /// etc.).
    /// </summary>
    /// <param name="tileSet">TileSet with atlas that should include all sprites.</param>
    public static void Initialize(TileSet tileSet)
    {
        NullGuard.ThrowIfNull(tileSet);

        lock (_lock)
        {
            _instance ??= new ArenaBuilder(tileSet);
        }
    }

    /// <summary>
    /// Draws a new platform on the arena in specified location.
    /// </summary>
    /// <remarks>
    /// The shortest platform is 2 tiles long, which will always draw Left and Right. <c>length</c> parameter specifies
    /// how many additional tiles to draw, which extend the platform.
    /// </remarks>
    /// <param name="location">Starting point where the platform is drawn (starting from Left).</param>
    /// <param name="length">How many Middle tiles to insert.</param>
    /// <param name="drawWith">Type of the platform to draw.</param>
    public void DrawPlatform(Vector2I location, int length, GameTileTypes drawWith = GameTileTypes.PlatformStone)
    {
        // Retrieves the default platform for drawing unless specified otherwise
        // BasePlatform platform = _platforms[drawWith];
        int start = location.X;
        int end = location.X + length;

        for (int x = start; start < end; x++)
        {
            // TBA
        }
    }

    /// <summary>
    /// Draws a single sprite on the TileMap at given location.
    /// </summary>
    /// <param name="location">Coordinates (location) on TileMap to draw at.</param>
    public void DrawPoint(Vector2I location)
    {
        // TBA
        GD.Print(_tileSet);
    }

    /// <summary>
    /// Draws a set of sprites on the TileMap as a horizontal line of specified length.
    /// </summary>
    /// <param name="location">Starting point where the line should be drawn from.</param>
    /// <param name="length">Length of the line to draw. If the length is 0, only Left+Right is drawn.</param>
    public void DrawLineHorizontally(Vector2I location, int length)
    {
        // TBA
    }

    public void DrawSquare(Vector2I location, int size, bool shouldFill = false)
    {
        // TBA
    }

    public void DrawRectangle(Vector2I startingPoint, Vector2I endingPoint, bool shouldFill = false)
    {
        // TBA
    }
}
