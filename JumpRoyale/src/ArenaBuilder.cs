using System;
using System.Collections.Generic;
using Godot;

namespace JumpRoyale;

public class ArenaBuilder
{
    private static readonly object _lock = new();

    private static ArenaBuilder? _instance;

    private readonly TileSet _tileSet;

    /// <summary>
    /// Collection of all Platforms that can be used for drawing one-way collision platforms.
    /// </summary>
    private readonly Dictionary<HorizontalTileTypes, BaseHorizontalObject> _horizontalObjects = [];
    private readonly Dictionary<HorizontalTileTypes, BaseVerticalObject> _verticalObjects = [];
    private readonly Dictionary<HorizontalTileTypes, BaseSingleBlock> _blocks = [];

    private ArenaBuilder(TileSet tileSet)
    {
        _tileSet = tileSet;
        TileMap = new() { Name = "TileMap", TileSet = _tileSet };

        // Store all Drawable objects
        _horizontalObjects.Add(HorizontalTileTypes.PlatformConcrete, GameTiles.PlatformConcrete);
        _horizontalObjects.Add(HorizontalTileTypes.PlatformGold, GameTiles.PlatformGold);
        _horizontalObjects.Add(HorizontalTileTypes.PlatformStone, GameTiles.PlatformStone);
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
    /// how many additional tiles to draw, which extend the object.
    /// </remarks>
    /// <param name="location">Starting point where the object is drawn (starting from Left).</param>
    /// <param name="length">How many Middle tiles to insert.</param>
    /// <param name="drawWith">Type of the object to draw.</param>
    public void DrawHorizontal(
        Vector2I location,
        int length,
        HorizontalTileTypes drawWith = HorizontalTileTypes.PlatformStone
    )
    {
        // Retrieves the default object for drawing unless specified otherwise
        BaseHorizontalObject platform = _horizontalObjects[drawWith];

        DrawCell(location, platform.Left);

        // Store the cell X where the right edge of the platform will be drawn
        Vector2I end = location + Vector2I.Right * (1 + length);

        // WIll only loop the middle part as long as the length is greater than 0
        for (int x = location.X; x < end.X; x++)
        {
            DrawCell(new Vector2I(x, location.Y), platform.Middle);
        }

        DrawCell(end, platform.Right);
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
