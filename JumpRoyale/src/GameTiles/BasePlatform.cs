using Godot;

namespace JumpRoyale;

public abstract class BasePlatform(Vector2I left, Vector2I middle, Vector2I right)
{
    /// <summary>
    /// Starting point where the platform will be drawn from. This tile will be inserted automatically.
    /// </summary>
    public Vector2I Left { get; } = left;

    /// <summary>
    /// Platform "extension", which will be drawn between <c>Left</c> and <c>Right</c> tiles.
    /// </summary>
    public Vector2I Middle { get; } = middle;

    /// <summary>
    /// Ending point where the last part of platform will be drawn. This tile will be inserted automatically.
    /// </summary>
    public Vector2I Right { get; } = right;
}
