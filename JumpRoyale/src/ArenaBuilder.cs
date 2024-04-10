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
    /// Sprites will change every [Y] height in tiles, defined by this value.
    /// </summary>
    private readonly int _tileChangeHeightFactor;

    /// <summary>
    /// Defines an array of tile types that will be used to draw a default tile depending on the current height. The
    /// order of the defined elements is important, they will be drawn in this order.
    /// </summary>
    private readonly TileTypes[] _nextTilesByHeight = [TileTypes.Stone, TileTypes.Concrete, TileTypes.Gold];

    /// <summary>
    /// Initializes a new instance of the <see cref="ArenaBuilder"/> class.
    /// </summary>
    /// <param name="tileMap">Tilemap to draw on.</param>
    /// <param name="tileChangeHeightFactor">How many tiles vertically it takes to change to the next sprite.</param>
    public ArenaBuilder(TileMap tileMap, int tileChangeHeightFactor)
    {
        TileMap = tileMap;
        _tileChangeHeightFactor = tileChangeHeightFactor;

        // Store all Drawable objects
        _horizontalPlatforms.Add(TileTypes.Stone, GameTiles.PlatformStone);
        _horizontalWalls.Add(TileTypes.Stone, GameTiles.HorizontalWallStone);
        _verticalWalls.Add(TileTypes.Stone, GameTiles.VerticalWallStone);
        _blocks.Add(TileTypes.Stone, GameTiles.BlockStone);

        _horizontalPlatforms.Add(TileTypes.Concrete, GameTiles.PlatformConcrete);
        _horizontalWalls.Add(TileTypes.Concrete, GameTiles.HorizontalWallConcrete);
        _verticalWalls.Add(TileTypes.Concrete, GameTiles.VerticalWallConcrete);
        _blocks.Add(TileTypes.Concrete, GameTiles.BlockConcrete);

        _horizontalPlatforms.Add(TileTypes.Gold, GameTiles.PlatformGold);
        _horizontalWalls.Add(TileTypes.Gold, GameTiles.HorizontalWallGold);
        _verticalWalls.Add(TileTypes.Gold, GameTiles.VerticalWallGold);
        _blocks.Add(TileTypes.Gold, GameTiles.BlockGold);
    }

    public TileMap TileMap { get; }

    public TileTypes TileTypeByIndex(int index)
    {
        return _nextTilesByHeight[Math.Clamp(index, 0, _nextTilesByHeight.Length - 1)];
    }

    public void DrawHorizontalPlatform(Vector2I startingPoint, int length, TileTypes? drawWith = null)
    {
        DrawHorizontally(startingPoint, length, _horizontalPlatforms[drawWith ?? TileTypeByHeight(startingPoint.Y)]);
    }

    public void DrawHorizontalWall(Vector2I startingPoint, int length, TileTypes? drawWith = null)
    {
        DrawHorizontally(startingPoint, length, _horizontalWalls[drawWith ?? TileTypeByHeight(startingPoint.Y)]);
    }

    public void DrawVerticalWall(Vector2I startingPoint, int height, TileTypes? drawWith = null)
    {
        DrawVertically(startingPoint, height, _verticalWalls[drawWith ?? TileTypeByHeight(startingPoint.Y)]);
    }

    public void DrawPoint(Vector2I location, TileTypes? drawWith = null)
    {
        DrawCell(location, _blocks[drawWith ?? TileTypeByHeight(location.Y)].SpriteLocation);
    }

    public void DrawSquare(
        Vector2I bottomLeft,
        int size,
        TileTypes? drawWith = null,
        bool shouldFill = true,
        TileTypes? fillWith = null
    )
    {
        TileTypes drawingTile = drawWith ?? TileTypeByHeight(bottomLeft.Y);
        TileTypes fillingTile = fillWith ?? TileTypeByHeight(bottomLeft.Y);

        if (size == 0)
        {
            DrawCell(bottomLeft, _blocks[drawingTile].SpriteLocation);

            return;
        }

        Vector2I endingPoint = new(bottomLeft.X + size, bottomLeft.Y - size);

        DrawRectangle(bottomLeft, endingPoint, drawingTile, shouldFill, fillingTile);
    }

    public void DrawBox(
        Vector2I bottomLeft,
        Vector2I topRight,
        TileTypes? drawWith = null,
        bool shouldFill = true,
        TileTypes? fillWith = null
    )
    {
        TileTypes drawingTile = drawWith ?? TileTypeByHeight(bottomLeft.Y);
        TileTypes fillingTile = fillWith ?? TileTypeByHeight(bottomLeft.Y);

        if (topRight.X < bottomLeft.X || topRight.Y > bottomLeft.Y)
        {
            throw new Exception("Boxes can only be drawn left-to-right, bottom-to-top");
        }

        DrawRectangle(bottomLeft, topRight, drawingTile, shouldFill, fillingTile);
    }

    public void EraseSpritesAtArea(Vector2I bottomLeft, Vector2I topRight)
    {
        for (int y = bottomLeft.Y; y >= topRight.Y; y--)
        {
            for (int x = bottomLeft.X; x <= topRight.X; x++)
            {
                EraseCellAt(new(x, y));
            }
        }
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

    /// <summary>
    /// Draws on the TileMap at given location with sprite defined at <c>atlasCoords</c>.
    /// </summary>
    /// <param name="drawAt">Location to draw the cell at.</param>
    /// <param name="atlasCoords">Sprite location on the TileMap.</param>
    private void DrawCell(Vector2I drawAt, Vector2I atlasCoords)
    {
        // NOTE: This could potentially perform a check if the cell is already occupied if we don't want to override the
        // previously drawn sprite
        TileMap.SetCell(0, drawAt, 0, atlasCoords);
    }

    /// <summary>
    /// Erases TileMap Cell at given location.
    /// </summary>
    /// <param name="location">TileMap coords to remove the cell at (set to empty).</param>
    private void EraseCellAt(Vector2I location)
    {
        TileMap.SetCell(0, location, 0, new(-1, -1));
    }

    /// <summary>
    /// Returns the tile type indexed by the amount of Height Factors in the current Y.
    /// </summary>
    private TileTypes TileTypeByHeight(int currentY)
    {
        int tileIndex = Math.Abs(currentY / _tileChangeHeightFactor);

        // On some arena maxHeight values, this will evaluate to +1 above the limit, so we have to clamp the index
        tileIndex = Math.Clamp(tileIndex, 0, _nextTilesByHeight.Length - 1);

        return _nextTilesByHeight[tileIndex];
    }
}
