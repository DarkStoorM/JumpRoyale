using Godot;
using JumpRoyale.Utils.Exceptions;

namespace JumpRoyale;

public partial class DrawingTestScene : Node2D
{
    private ArenaBuilder _builder = null!;

    [Export]
    public TileSet? TileSetToUse { get; private set; }

    public override void _Ready()
    {
        TileMap tileMap = GetNode<TileMap>("TileMap");

        if (tileMap.TileSet is null)
        {
            throw new UnassignedSceneOrComponentException();
        }

        _builder = new ArenaBuilder(tileMap, 9999);

        DrawTestPoints();
        DrawTestPlatforms();
        DrawTestWalls();
        DrawTestSquares();
        DrawTestRectangles();
    }

    private void DrawTestPoints()
    {
        _builder.DrawPoint(new Vector2I(15, 3));
        _builder.DrawPoint(new Vector2I(17, 3), TileTypes.Concrete);
        _builder.DrawPoint(new Vector2I(19, 3), TileTypes.Gold);
    }

    private void DrawTestPlatforms()
    {
        _builder.DrawHorizontalPlatform(new Vector2I(10, 7), 0);
        _builder.DrawHorizontalPlatform(new Vector2I(15, 7), 0, TileTypes.Concrete);
        _builder.DrawHorizontalPlatform(new Vector2I(20, 7), 0, TileTypes.Gold);

        _builder.DrawHorizontalPlatform(new Vector2I(10, 8), 1);
        _builder.DrawHorizontalPlatform(new Vector2I(15, 8), 1, TileTypes.Concrete);
        _builder.DrawHorizontalPlatform(new Vector2I(20, 8), 1, TileTypes.Gold);

        _builder.DrawHorizontalPlatform(new Vector2I(10, 9), 2);
        _builder.DrawHorizontalPlatform(new Vector2I(15, 9), 2, TileTypes.Concrete);
        _builder.DrawHorizontalPlatform(new Vector2I(20, 9), 2, TileTypes.Gold);
    }

    private void DrawTestWalls()
    {
        _builder.DrawHorizontalWall(new Vector2I(10, 12), 0);
        _builder.DrawHorizontalWall(new Vector2I(15, 12), 0, TileTypes.Concrete);
        _builder.DrawHorizontalWall(new Vector2I(20, 12), 0, TileTypes.Gold);

        _builder.DrawHorizontalWall(new Vector2I(10, 13), 1);
        _builder.DrawHorizontalWall(new Vector2I(15, 13), 1, TileTypes.Concrete);
        _builder.DrawHorizontalWall(new Vector2I(20, 13), 1, TileTypes.Gold);

        _builder.DrawHorizontalWall(new Vector2I(10, 14), 2);
        _builder.DrawHorizontalWall(new Vector2I(15, 14), 2, TileTypes.Concrete);
        _builder.DrawHorizontalWall(new Vector2I(20, 14), 2, TileTypes.Gold);

        _builder.DrawVerticalWall(new Vector2I(10, 17), 0);
        _builder.DrawVerticalWall(new Vector2I(11, 18), 1);
        _builder.DrawVerticalWall(new Vector2I(12, 19), 2);

        _builder.DrawVerticalWall(new Vector2I(15, 17), 0, TileTypes.Concrete);
        _builder.DrawVerticalWall(new Vector2I(16, 18), 1, TileTypes.Concrete);
        _builder.DrawVerticalWall(new Vector2I(17, 19), 2, TileTypes.Concrete);

        _builder.DrawVerticalWall(new Vector2I(20, 17), 0, TileTypes.Gold);
        _builder.DrawVerticalWall(new Vector2I(21, 18), 1, TileTypes.Gold);
        _builder.DrawVerticalWall(new Vector2I(22, 19), 2, TileTypes.Gold);
    }

    private void DrawTestSquares()
    {
        _builder.DrawSquare(new Vector2I(1, 30), 0);
        _builder.DrawSquare(new Vector2I(1, 35), 0, TileTypes.Concrete);
        _builder.DrawSquare(new Vector2I(1, 40), 0, TileTypes.Gold);

        _builder.DrawSquare(new Vector2I(3, 31), 1);
        _builder.DrawSquare(new Vector2I(3, 36), 1, TileTypes.Concrete);
        _builder.DrawSquare(new Vector2I(3, 41), 1, TileTypes.Gold);

        _builder.DrawSquare(new Vector2I(6, 32), 2, shouldFill: false);
        _builder.DrawSquare(new Vector2I(6, 37), 2, TileTypes.Concrete, false);
        _builder.DrawSquare(new Vector2I(6, 42), 2, TileTypes.Gold, false);

        _builder.DrawSquare(new Vector2I(10, 33), 3, shouldFill: false);
        _builder.DrawSquare(new Vector2I(10, 38), 3, TileTypes.Concrete, false);
        _builder.DrawSquare(new Vector2I(10, 43), 3, TileTypes.Gold, false);

        // Filled Squares
        _builder.DrawSquare(new Vector2I(15, 30), 0);
        _builder.DrawSquare(new Vector2I(15, 35), 0, TileTypes.Concrete, true, TileTypes.Concrete);
        _builder.DrawSquare(new Vector2I(15, 40), 0, TileTypes.Gold, true, TileTypes.Gold);

        _builder.DrawSquare(new Vector2I(17, 31), 1);
        _builder.DrawSquare(new Vector2I(17, 36), 1, TileTypes.Concrete, true, TileTypes.Concrete);
        _builder.DrawSquare(new Vector2I(17, 41), 1, TileTypes.Gold, true, TileTypes.Gold);

        _builder.DrawSquare(new Vector2I(20, 32), 2);
        _builder.DrawSquare(new Vector2I(20, 37), 2, TileTypes.Concrete, true, TileTypes.Concrete);
        _builder.DrawSquare(new Vector2I(20, 42), 2, TileTypes.Gold, true, TileTypes.Gold);

        _builder.DrawSquare(new Vector2I(24, 33), 3);
        _builder.DrawSquare(new Vector2I(24, 38), 3, TileTypes.Concrete, true, TileTypes.Concrete);
        _builder.DrawSquare(new Vector2I(24, 43), 3, TileTypes.Gold, true, TileTypes.Gold);

        // Different filling
        _builder.DrawSquare(new Vector2I(29, 30), 0);
        _builder.DrawSquare(new Vector2I(29, 35), 0, TileTypes.Concrete, true, TileTypes.Concrete);
        _builder.DrawSquare(new Vector2I(29, 40), 0, TileTypes.Gold, true, TileTypes.Gold);

        _builder.DrawSquare(new Vector2I(31, 31), 1);
        _builder.DrawSquare(new Vector2I(31, 36), 1, TileTypes.Concrete, true, TileTypes.Concrete);
        _builder.DrawSquare(new Vector2I(31, 41), 1, TileTypes.Gold, true, TileTypes.Gold);

        _builder.DrawSquare(new Vector2I(34, 32), 2, TileTypes.Stone, true, TileTypes.Concrete);
        _builder.DrawSquare(new Vector2I(34, 37), 2, TileTypes.Concrete, true, TileTypes.Gold);
        _builder.DrawSquare(new Vector2I(34, 42), 2, TileTypes.Gold, true, TileTypes.Stone);

        _builder.DrawSquare(new Vector2I(38, 33), 3, TileTypes.Stone, true, TileTypes.Concrete);
        _builder.DrawSquare(new Vector2I(38, 38), 3, TileTypes.Concrete, true, TileTypes.Gold);
        _builder.DrawSquare(new Vector2I(38, 43), 3, TileTypes.Gold, true, TileTypes.Stone);
    }

    private void DrawTestRectangles()
    {
        _builder.DrawBox(new Vector2I(44, 30), new Vector2I(44, 30));
        _builder.DrawBox(new Vector2I(44, 35), new Vector2I(44, 35), TileTypes.Concrete, false);
        _builder.DrawBox(new Vector2I(44, 40), new Vector2I(44, 40), TileTypes.Gold, false);

        _builder.DrawBox(new Vector2I(46, 31), new Vector2I(48, 30));
        _builder.DrawBox(new Vector2I(46, 36), new Vector2I(48, 35), TileTypes.Concrete, false);
        _builder.DrawBox(new Vector2I(46, 41), new Vector2I(48, 40), TileTypes.Gold, false);

        _builder.DrawBox(new Vector2I(50, 32), new Vector2I(53, 30), shouldFill: false);
        _builder.DrawBox(new Vector2I(50, 37), new Vector2I(53, 35), TileTypes.Concrete, false);
        _builder.DrawBox(new Vector2I(50, 42), new Vector2I(53, 40), TileTypes.Gold, false);

        _builder.DrawBox(new Vector2I(55, 33), new Vector2I(57, 30), shouldFill: false);
        _builder.DrawBox(new Vector2I(55, 38), new Vector2I(57, 35), TileTypes.Concrete, false);
        _builder.DrawBox(new Vector2I(55, 43), new Vector2I(57, 40), TileTypes.Gold, false);

        // Filled
        _builder.DrawBox(new Vector2I(59, 30), new Vector2I(59, 30));
        _builder.DrawBox(new Vector2I(59, 35), new Vector2I(59, 35), TileTypes.Concrete, true, TileTypes.Concrete);
        _builder.DrawBox(new Vector2I(59, 40), new Vector2I(59, 40), TileTypes.Gold, true, TileTypes.Gold);

        _builder.DrawBox(new Vector2I(61, 31), new Vector2I(63, 30));
        _builder.DrawBox(new Vector2I(61, 36), new Vector2I(63, 35), TileTypes.Concrete, true, TileTypes.Concrete);
        _builder.DrawBox(new Vector2I(61, 41), new Vector2I(63, 40), TileTypes.Gold, true, TileTypes.Gold);

        _builder.DrawBox(new Vector2I(65, 32), new Vector2I(68, 30));
        _builder.DrawBox(new Vector2I(65, 37), new Vector2I(68, 35), TileTypes.Concrete, true, TileTypes.Concrete);
        _builder.DrawBox(new Vector2I(65, 42), new Vector2I(68, 40), TileTypes.Gold, true, TileTypes.Gold);

        _builder.DrawBox(new Vector2I(70, 33), new Vector2I(72, 30));
        _builder.DrawBox(new Vector2I(70, 38), new Vector2I(72, 35), TileTypes.Concrete, true, TileTypes.Concrete);
        _builder.DrawBox(new Vector2I(70, 43), new Vector2I(72, 40), TileTypes.Gold, true, TileTypes.Gold);

        // Different filling
        _builder.DrawBox(new Vector2I(74, 30), new Vector2I(74, 30));
        _builder.DrawBox(new Vector2I(74, 35), new Vector2I(74, 35), TileTypes.Concrete);
        _builder.DrawBox(new Vector2I(74, 40), new Vector2I(74, 40), TileTypes.Gold);

        _builder.DrawBox(new Vector2I(76, 31), new Vector2I(78, 30));
        _builder.DrawBox(new Vector2I(76, 36), new Vector2I(78, 35), TileTypes.Concrete);
        _builder.DrawBox(new Vector2I(76, 41), new Vector2I(78, 40), TileTypes.Gold);

        _builder.DrawBox(new Vector2I(80, 32), new Vector2I(83, 30), TileTypes.Stone, true, TileTypes.Concrete);
        _builder.DrawBox(new Vector2I(80, 37), new Vector2I(83, 35), TileTypes.Concrete, true, TileTypes.Gold);
        _builder.DrawBox(new Vector2I(80, 42), new Vector2I(83, 40), TileTypes.Gold, true, TileTypes.Stone);

        _builder.DrawBox(new Vector2I(85, 33), new Vector2I(87, 30), TileTypes.Stone, true, TileTypes.Concrete);
        _builder.DrawBox(new Vector2I(85, 38), new Vector2I(87, 35), TileTypes.Concrete, true, TileTypes.Gold);
        _builder.DrawBox(new Vector2I(85, 43), new Vector2I(87, 40), TileTypes.Gold, true, TileTypes.Stone);
    }
}
