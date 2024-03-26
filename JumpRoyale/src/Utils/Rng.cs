using System;

namespace JumpRoyale.Utils;

public static class Rng
{
    private static readonly Random _rng = new();

    /// <summary>
    /// Returns a random integer from specified range (both min and max are inclusive).
    /// </summary>
    public static int IntRange(int min, int max)
    {
        return _rng.Next(min, max);
    }

    public static bool RandomBool()
    {
        return _rng.NextSingle() < 0.5;
    }

    public static int RandomInt()
    {
        return _rng.Next();
    }

    public static float RandomFloat()
    {
        return _rng.NextSingle();
    }

    /// <summary>
    /// Returns a random Godot Color.
    /// </summary>
    public static string RandomHex()
    {
        return new Godot.Color(_rng.NextSingle(), _rng.NextSingle(), _rng.NextSingle(), 1).ToHtml(false);
    }
}
