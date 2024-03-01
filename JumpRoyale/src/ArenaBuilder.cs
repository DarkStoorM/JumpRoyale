using System;
using System.Collections.Generic;
using Godot;

namespace JumpRoyale;

public class ArenaBuilder
{
    private static readonly object _lock = new();

    private static ArenaBuilder? _instance;

    /// <summary>
    /// Collection of all Platforms that can be used for drawing one-way collision platforms.
    /// </summary>
    private readonly Dictionary<TileTypes, BaseHorizontalObject> _horizontalPlatforms = [];
    private readonly Dictionary<TileTypes, BaseHorizontalObject> _horizontalWalls = [];
    private readonly Dictionary<TileTypes, BaseVerticalObject> _verticalWalls = [];
    private readonly Dictionary<TileTypes, BaseSingleBlock> _blocks = [];

    private ArenaBuilder(TileSet tileSet)
    {
        TileMap = new() { Name = "TileMap", TileSet = tileSet };

        // Store all Drawable objects
        _horizontalPlatforms.Add(TileTypes.Concrete, GameTiles.PlatformConcrete);
        _horizontalPlatforms.Add(TileTypes.Gold, GameTiles.PlatformGold);
        _horizontalPlatforms.Add(TileTypes.Stone, GameTiles.PlatformStone);

        _horizontalWalls.Add(TileTypes.Concrete, GameTiles.HorizontalWallConcrete);
        _horizontalWalls.Add(TileTypes.Gold, GameTiles.HorizontalWallGold);
        _horizontalWalls.Add(TileTypes.Stone, GameTiles.HorizontalWallStone);

        _verticalWalls.Add(TileTypes.Concrete, GameTiles.VerticalWallConcrete);
        _verticalWalls.Add(TileTypes.Gold, GameTiles.VerticalWallGold);
        _verticalWalls.Add(TileTypes.Stone, GameTiles.VerticalWallStone);

        _blocks.Add(TileTypes.Stone, GameTiles.BlockConcrete);
        _blocks.Add(TileTypes.Stone, GameTiles.BlockGold);
        _blocks.Add(TileTypes.Stone, GameTiles.BlockStone);
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

    public TileMap TileMap { get; }

    /// <summary>
    /// Creates ArenaBuilder instance with TileSet containing sprites to assign to platforms and other objects (walls,
    /// etc.).
    /// </summary>
    /// <param name="tileSet">TileSet with atlas that should include all sprites.</param>
    public static void Initialize(TileSet tileSet)
    {
        lock (_lock)
        {
            _instance ??= new ArenaBuilder(tileSet);
        }
    }

    /// <summary>
    /// Draws a new horizontal object on the arena in specified location.
    /// </summary>
    /// <remarks>
    /// The shortest object is 2 tiles long, which will always draw Left and Right. <c>length</c> parameter specifies
    /// how many additional tiles to draw, which extend the object, so despite providing the length of 0, the object
    /// will always be drawn.
    /// </remarks>
    /// <param name="location">Starting point where the object is drawn (starting from Left).</param>
    /// <param name="length">How many Middle tiles to insert.</param>
    /// <param name="drawWith">Type of the object to draw.</param>
    public void DrawHorizontalPlatform(Vector2I location, int length, TileTypes drawWith = TileTypes.Stone)
    {
        // Retrieves the default object for drawing unless specified otherwise
        BaseHorizontalObject platform = _horizontalPlatforms[drawWith];

        DrawCell(location, platform.Left);

        // Store the cell X where the right edge of the platform will be drawn
        Vector2I end = location + Vector2I.Right * (1 + length);

        // Will only loop the middle part as long as the length is greater than 0
        for (int x = location.X; x < end.X; x++)
        {
            DrawCell(new Vector2I(x, location.Y), platform.Middle);
        }

        DrawCell(end, platform.Right);
    }

    /// <summary>
    /// Work-in-progress, requires extraction later.
    /// </summary>
    public void DrawHorizontalWall(Vector2I location, int length, TileTypes drawWith = TileTypes.Stone)
    {
        // Retrieves the default object for drawing unless specified otherwise
        BaseHorizontalObject wall = _horizontalWalls[drawWith];

        DrawCell(location, wall.Left);

        // Store the cell X where the right edge of the platform will be drawn
        Vector2I end = location + Vector2I.Right * (1 + length);

        // Will only loop the middle part as long as the length is greater than 0
        for (int x = location.X; x < end.X; x++)
        {
            DrawCell(new Vector2I(x, location.Y), wall.Middle);
        }

        DrawCell(end, wall.Right);
    }

    /// <summary>
    /// Draws a single sprite on the TileMap at given location.
    /// </summary>
    /// <param name="location">Coordinates (location) on TileMap to draw at.</param>
    public void DrawPoint(Vector2I location, TileTypes drawWith = TileTypes.Stone)
    {
        BaseSingleBlock block = _blocks[drawWith];

        DrawCell(location, block.SpriteLocation);
    }

    public void DrawSquare(Vector2I location, int size, bool shouldFill = false)
    {
        // TBA
    }

    public void DrawRectangle(Vector2I startingPoint, Vector2I endingPoint, bool shouldFill = false)
    {
        // TBA
    }

    private void DrawCell(Vector2I location, Vector2I atlasCoords)
    {
        TileMap.SetCell(0, location, 0, atlasCoords);
    }
}
