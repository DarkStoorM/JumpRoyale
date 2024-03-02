using System.Collections.Generic;
using Godot;

namespace JumpRoyale;

public class ArenaBuilder : IArenaBuilder
{
    private readonly Dictionary<TileTypes, BaseLineObject> _horizontalPlatforms = [];
    private readonly Dictionary<TileTypes, BaseLineObject> _horizontalWalls = [];
    private readonly Dictionary<TileTypes, BaseLineObject> _verticalWalls = [];
    private readonly Dictionary<TileTypes, BasePointObject> _blocks = [];

    public ArenaBuilder(TileSet tileSet)
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

        _blocks.Add(TileTypes.Concrete, GameTiles.BlockConcrete);
        _blocks.Add(TileTypes.Gold, GameTiles.BlockGold);
        _blocks.Add(TileTypes.Stone, GameTiles.BlockStone);
    }

    public TileMap TileMap { get; }

    public void DrawHorizontalPlatform(Vector2I location, int length, TileTypes drawWith = TileTypes.Stone)
    {
        DrawHorizontally(location, length, _horizontalPlatforms[drawWith]);
    }

    public void DrawHorizontalWall(Vector2I location, int length, TileTypes drawWith = TileTypes.Stone)
    {
        DrawHorizontally(location, length, _horizontalWalls[drawWith]);
    }

    public void DrawVerticalWall(Vector2I location, int height, TileTypes drawWith = TileTypes.Stone)
    {
        DrawVertically(location, height, _verticalWalls[drawWith]);
    }

    public void DrawPoint(Vector2I location, TileTypes drawWith = TileTypes.Stone)
    {
        DrawCell(location, _blocks[drawWith].SpriteLocation);
    }

    public void DrawSquare(Vector2I location, int size, bool shouldFill = false)
    {
        // TBA
    }

    public void DrawRectangle(Vector2I startingPoint, Vector2I endingPoint, bool shouldFill = false)
    {
        // TBA
    }

    /// <summary>
    /// Draws a horizontal line on the TileMap with specified object and given length.
    /// </summary>
    private void DrawHorizontally(Vector2I location, int length, BaseLineObject obj)
    {
        DrawCell(location, obj.Start);

        // Store the cell X where the finish point of the object will be drawn
        Vector2I end = location + Vector2I.Right * (1 + length);

        // Will only loop the middle part as long as the length is greater than 0
        for (int x = location.X + 1; x < end.X; x++)
        {
            DrawCell(new Vector2I(x, location.Y), obj.Middle);
        }

        DrawCell(end, obj.Finish);
    }

    /// <summary>
    /// Draws a Vertical line on the TileMap with specified object and given length.
    /// </summary>
    private void DrawVertically(Vector2I location, int length, BaseLineObject obj)
    {
        DrawCell(location, obj.Start);

        // Store the cell Y where the finish point of the object will be drawn
        Vector2I end = location + Vector2I.Down * (1 + length);

        // Will only loop the middle part as long as the length is greater than 0
        for (int y = location.Y + 1; y < end.Y; y++)
        {
            DrawCell(new Vector2I(location.X, y), obj.Middle);
        }

        DrawCell(end, obj.Finish);
    }

    private void DrawCell(Vector2I location, Vector2I atlasCoords)
    {
        TileMap.SetCell(0, location, 0, atlasCoords);
    }
}
