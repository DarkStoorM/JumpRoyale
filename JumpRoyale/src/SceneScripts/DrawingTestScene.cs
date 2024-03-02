using Godot;
using JumpRoyale.Utils;

namespace JumpRoyale;

public partial class DrawingTestScene : Node2D
{
    private ArenaBuilder _builder = null!;

    [Export]
    public TileSet? TileSetToUse { get; private set; }

    public override void _Ready()
    {
        NullGuard.ThrowIfNull(TileSetToUse);

        _builder = new ArenaBuilder(TileSetToUse);

        AddChild(_builder.TileMap);

        DrawTestPoints();
        DrawTestPlatforms();
        DrawTestWalls();
        DrawTestSquares();
        DrawTestRectangles();
    }

    private void DrawTestPoints()
    {
        _builder.DrawPoint(new Vector2I(15, 3), TileTypes.Stone);
        _builder.DrawPoint(new Vector2I(17, 3), TileTypes.Concrete);
        _builder.DrawPoint(new Vector2I(19, 3), TileTypes.Gold);
    }

    private void DrawTestPlatforms()
    {
        _builder.DrawHorizontalPlatform(new Vector2I(10, 7), 0, TileTypes.Stone);
        _builder.DrawHorizontalPlatform(new Vector2I(15, 7), 0, TileTypes.Concrete);
        _builder.DrawHorizontalPlatform(new Vector2I(20, 7), 0, TileTypes.Gold);

        _builder.DrawHorizontalPlatform(new Vector2I(10, 8), 1, TileTypes.Stone);
        _builder.DrawHorizontalPlatform(new Vector2I(15, 8), 1, TileTypes.Concrete);
        _builder.DrawHorizontalPlatform(new Vector2I(20, 8), 1, TileTypes.Gold);

        _builder.DrawHorizontalPlatform(new Vector2I(10, 9), 2, TileTypes.Stone);
        _builder.DrawHorizontalPlatform(new Vector2I(15, 9), 2, TileTypes.Concrete);
        _builder.DrawHorizontalPlatform(new Vector2I(20, 9), 2, TileTypes.Gold);
    }

    private void DrawTestWalls()
    {
        _builder.DrawHorizontalWall(new Vector2I(10, 12), 0, TileTypes.Stone);
        _builder.DrawHorizontalWall(new Vector2I(15, 12), 0, TileTypes.Concrete);
        _builder.DrawHorizontalWall(new Vector2I(20, 12), 0, TileTypes.Gold);

        _builder.DrawHorizontalWall(new Vector2I(10, 13), 1, TileTypes.Stone);
        _builder.DrawHorizontalWall(new Vector2I(15, 13), 1, TileTypes.Concrete);
        _builder.DrawHorizontalWall(new Vector2I(20, 13), 1, TileTypes.Gold);

        _builder.DrawHorizontalWall(new Vector2I(10, 14), 2, TileTypes.Stone);
        _builder.DrawHorizontalWall(new Vector2I(15, 14), 2, TileTypes.Concrete);
        _builder.DrawHorizontalWall(new Vector2I(20, 14), 2, TileTypes.Gold);

        _builder.DrawVerticalWall(new Vector2I(10, 16), 0, TileTypes.Stone);
        _builder.DrawVerticalWall(new Vector2I(11, 16), 1, TileTypes.Stone);
        _builder.DrawVerticalWall(new Vector2I(12, 16), 2, TileTypes.Stone);

        _builder.DrawVerticalWall(new Vector2I(15, 16), 0, TileTypes.Concrete);
        _builder.DrawVerticalWall(new Vector2I(16, 16), 1, TileTypes.Concrete);
        _builder.DrawVerticalWall(new Vector2I(17, 16), 2, TileTypes.Concrete);

        _builder.DrawVerticalWall(new Vector2I(21, 16), 0, TileTypes.Gold);
        _builder.DrawVerticalWall(new Vector2I(22, 16), 1, TileTypes.Gold);
        _builder.DrawVerticalWall(new Vector2I(23, 16), 2, TileTypes.Gold);
    }

    private void DrawTestSquares()
    {
        _builder.DrawSquare(new Vector2I(1, 30), 0, drawWith: TileTypes.Stone);
        _builder.DrawSquare(new Vector2I(1, 35), 0, drawWith: TileTypes.Concrete);
        _builder.DrawSquare(new Vector2I(1, 40), 0, drawWith: TileTypes.Gold);

        _builder.DrawSquare(new Vector2I(3, 31), 1, drawWith: TileTypes.Stone);
        _builder.DrawSquare(new Vector2I(3, 36), 1, drawWith: TileTypes.Concrete);
        _builder.DrawSquare(new Vector2I(3, 41), 1, drawWith: TileTypes.Gold);

        _builder.DrawSquare(new Vector2I(6, 32), 2, drawWith: TileTypes.Stone);
        _builder.DrawSquare(new Vector2I(6, 37), 2, drawWith: TileTypes.Concrete);
        _builder.DrawSquare(new Vector2I(6, 42), 2, drawWith: TileTypes.Gold);

        _builder.DrawSquare(new Vector2I(10, 33), 3, drawWith: TileTypes.Stone);
        _builder.DrawSquare(new Vector2I(10, 38), 3, drawWith: TileTypes.Concrete);
        _builder.DrawSquare(new Vector2I(10, 43), 3, drawWith: TileTypes.Gold);

        // Filled Squares
        _builder.DrawSquare(new Vector2I(15, 30), 0, true, TileTypes.Stone, TileTypes.Stone);
        _builder.DrawSquare(new Vector2I(15, 35), 0, true, TileTypes.Concrete, TileTypes.Concrete);
        _builder.DrawSquare(new Vector2I(15, 40), 0, true, TileTypes.Gold, TileTypes.Gold);

        _builder.DrawSquare(new Vector2I(17, 31), 1, true, TileTypes.Stone, TileTypes.Stone);
        _builder.DrawSquare(new Vector2I(17, 36), 1, true, TileTypes.Concrete, TileTypes.Concrete);
        _builder.DrawSquare(new Vector2I(17, 41), 1, true, TileTypes.Gold, TileTypes.Gold);

        _builder.DrawSquare(new Vector2I(20, 32), 2, true, TileTypes.Stone, TileTypes.Stone);
        _builder.DrawSquare(new Vector2I(20, 37), 2, true, TileTypes.Concrete, TileTypes.Concrete);
        _builder.DrawSquare(new Vector2I(20, 42), 2, true, TileTypes.Gold, TileTypes.Gold);

        _builder.DrawSquare(new Vector2I(24, 33), 3, true, TileTypes.Stone, TileTypes.Stone);
        _builder.DrawSquare(new Vector2I(24, 38), 3, true, TileTypes.Concrete, TileTypes.Concrete);
        _builder.DrawSquare(new Vector2I(24, 43), 3, true, TileTypes.Gold, TileTypes.Gold);

        // DIfferent filling
        _builder.DrawSquare(new Vector2I(29, 30), 0, true, TileTypes.Stone, TileTypes.Stone);
        _builder.DrawSquare(new Vector2I(29, 35), 0, true, TileTypes.Concrete, TileTypes.Concrete);
        _builder.DrawSquare(new Vector2I(29, 40), 0, true, TileTypes.Gold, TileTypes.Gold);

        _builder.DrawSquare(new Vector2I(31, 31), 1, true, TileTypes.Stone, TileTypes.Stone);
        _builder.DrawSquare(new Vector2I(31, 36), 1, true, TileTypes.Concrete, TileTypes.Concrete);
        _builder.DrawSquare(new Vector2I(31, 41), 1, true, TileTypes.Gold, TileTypes.Gold);

        _builder.DrawSquare(new Vector2I(34, 32), 2, true, TileTypes.Stone, TileTypes.Concrete);
        _builder.DrawSquare(new Vector2I(34, 37), 2, true, TileTypes.Concrete, TileTypes.Gold);
        _builder.DrawSquare(new Vector2I(34, 42), 2, true, TileTypes.Gold, TileTypes.Stone);

        _builder.DrawSquare(new Vector2I(38, 33), 3, true, TileTypes.Stone, TileTypes.Concrete);
        _builder.DrawSquare(new Vector2I(38, 38), 3, true, TileTypes.Concrete, TileTypes.Gold);
        _builder.DrawSquare(new Vector2I(38, 43), 3, true, TileTypes.Gold, TileTypes.Stone);
    }

    private void DrawTestRectangles() { }
}
