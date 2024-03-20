using System;
using System.Collections.Generic;
using Godot;

namespace JumpRoyale;

public class ArenaBuilder : IArenaBuilder
{
    private readonly Dictionary<TileTypes, BaseLineObject> _horizontalPlatforms = [];
    private readonly Dictionary<TileTypes, BaseLineObject> _horizontalWalls = [];
    private readonly Dictionary<TileTypes, BaseLineObject> _verticalWalls = [];
    private readonly Dictionary<TileTypes, BasePointObject> _blocks = [];

    /// <summary>
    /// Sprites will change every [Y] height, defined by this value.
    /// </summary>
    private readonly int _tileChangeHeightFactor;

    /// <summary>
    /// Defines an array of tile types that will be used to draw a default tile depending on the current height. The
    /// order of the defined elements is important, they will be drawn in this order.
    /// </summary>
    private readonly TileTypes[] _nextTilesByHeight = [TileTypes.Stone, TileTypes.Concrete, TileTypes.Gold];

    public ArenaBuilder(TileMap tileMap, int tileChangeHeightFactor)
    {
        TileMap = tileMap;
        _tileChangeHeightFactor = tileChangeHeightFactor;

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

    public void DrawHorizontalPlatform(Vector2I location, int length, TileTypes? drawWith = null)
    {
        DrawHorizontally(location, length, _horizontalPlatforms[drawWith ?? TileTypeByHeight(location.Y)]);
    }

    public void DrawHorizontalWall(Vector2I location, int length, TileTypes? drawWith = null)
    {
        DrawHorizontally(location, length, _horizontalWalls[drawWith ?? TileTypeByHeight(location.Y)]);
    }

    public void DrawVerticalWall(Vector2I location, int height, TileTypes? drawWith = null)
    {
        DrawVertically(location, height, _verticalWalls[drawWith ?? TileTypeByHeight(location.Y)]);
    }

    public void DrawPoint(Vector2I location, TileTypes? drawWith = null)
    {
        DrawCell(location, _blocks[drawWith ?? TileTypeByHeight(location.Y)].SpriteLocation);
    }

    public void DrawSquare(
        Vector2I location,
        int size,
        TileTypes? drawWith = null,
        bool shouldFill = true,
        TileTypes? fillWith = null
    )
    {
        TileTypes drawingTile = drawWith ?? TileTypeByHeight(location.Y);
        TileTypes fillingTile = fillWith ?? TileTypeByHeight(location.Y);

        if (size == 0)
        {
            DrawCell(location, _blocks[drawingTile].SpriteLocation);

            return;
        }

        Vector2I endingPoint = new(location.X + size, location.Y - size);

        DrawRectangle(location, endingPoint, drawingTile, shouldFill, fillingTile);
    }

    public void DrawBox(
        Vector2I startingPoint,
        Vector2I endingPoint,
        TileTypes? drawWith = null,
        bool shouldFill = true,
        TileTypes? fillWith = null
    )
    {
        TileTypes drawingTile = drawWith ?? TileTypeByHeight(startingPoint.Y);
        TileTypes fillingTile = fillWith ?? TileTypeByHeight(startingPoint.Y);

        if (endingPoint.X < startingPoint.X || endingPoint.Y > startingPoint.Y)
        {
            throw new Exception("Boxes can only be drawn left-to-right, bottom-to-top");
        }

        DrawRectangle(startingPoint, endingPoint, drawingTile, shouldFill, fillingTile);
    }

    /// <summary>
    /// Shared logic for drawing horizontal Platforms and Walls.
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
    /// Shared logic for drawing vertical Walls.
    /// </summary>
    private void DrawVertically(Vector2I location, int length, BaseLineObject obj)
    {
        DrawCell(location, obj.Finish);

        // Store the cell Y where the finish point of the object will be drawn
        Vector2I end = new(location.X, location.Y - length - 1);

        // Will only loop the middle part as long as the length is greater than 0
        for (int y = location.Y - 1; y > end.Y - 1; y--)
        {
            DrawCell(new Vector2I(location.X, y), obj.Middle);
        }

        DrawCell(end, obj.Start);
    }

    /// <summary>
    /// Shared logic for drawing Squares and Boxes (variable size).
    /// </summary>
    /// <remarks>See <see cref="DrawSquare"/> for information on parameters and drawing method.</remarks>
    private void DrawRectangle(
        Vector2I startingPoint,
        Vector2I endingPoint,
        TileTypes drawWith = TileTypes.Stone,
        bool shouldFill = false,
        TileTypes? fillWith = null
    )
    {
        BasePointObject drawingObject = _blocks[drawWith];
        BasePointObject fillerObject = _blocks[fillWith ?? drawWith];

        // Always draw the first cell no matter what
        DrawCell(startingPoint, drawingObject.SpriteLocation);

        for (int y = startingPoint.Y; y > endingPoint.Y - 1; y--)
        {
            for (int x = startingPoint.X; x < endingPoint.X + 1; x++)
            {
                // Determine the drawing object. If we are on bounds, use primary. Secondary if "inside"
                bool isOnBounds =
                    x == startingPoint.X || x == endingPoint.X || y == startingPoint.Y || y == endingPoint.Y;

                // Ignore the drawing if we are inside the object, but didn't ask to fill it
                if (!shouldFill && !isOnBounds)
                {
                    continue;
                }

                // Select the drawing object depending on the position (bounds / inside)
                BasePointObject obj = isOnBounds ? drawingObject : fillerObject;

                DrawCell(new Vector2I(x, y), obj.SpriteLocation);
            }
        }
    }

    private void DrawCell(Vector2I location, Vector2I atlasCoords)
    {
        // NOTE: This could potentially perform a check if the cell is already occupied if we don't want to override the
        // previously drawn sprite
        TileMap.SetCell(0, location, 0, atlasCoords);
    }

    /// <summary>
    /// Returns the tile type indexed by the amount of Height Factors in the current Y.
    /// </summary>
    private TileTypes TileTypeByHeight(int currentY)
    {
        int tileIndex = Math.Clamp(currentY * 16 / _tileChangeHeightFactor, 0, _nextTilesByHeight.Length - 1);

        return _nextTilesByHeight[tileIndex];
    }
}
