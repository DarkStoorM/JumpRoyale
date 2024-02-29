using System;
using Godot;
using JumpRoyale.Utils;

namespace JumpRoyale;

public class ArenaBuilder
{
    private static readonly object _lock = new();
    private static ArenaBuilder? _instance;

    private readonly TileSet _tileSet;

    private ArenaBuilder(TileSet tileSet)
    {
        _tileSet = tileSet;
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

    public static void Initialize(TileSet tileSet)
    {
        NullGuard.ThrowIfNull(tileSet);

        lock (_lock)
        {
            _instance ??= new ArenaBuilder(tileSet);
        }
    }

    public void DrawPoint(Vector2 location)
    {
        // TBA
        GD.Print(_tileSet);
    }

    public void DrawLineHorizontally(Vector2 location, int length)
    {
        // TBA
    }

    public void DrawLineVertically(Vector2 location, int length)
    {
        // TBA
    }

    public void DrawSquare(Vector2 location, int size, bool shouldFill = false)
    {
        // TBA
    }

    public void DrawRectangle(Vector2 startingPoint, Vector2 endingPoint, bool shouldFill = false)
    {
        // TBA
    }
}
