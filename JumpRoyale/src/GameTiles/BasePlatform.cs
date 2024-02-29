using Godot;

namespace JumpRoyale;

public abstract class BasePlatform(Vector2I left, Vector2I middle, Vector2I right)
{
    /// <summary>
    /// Coordinates on the Atlas of the left edge of the platform.
    /// </summary>
    public Vector2I Left { get; } = left;

    /// <summary>
    /// Coordinates on the Atlas of the middle part of the platform. This part is used to draw the "extension" of a
    /// drawn platform. This should represent a seamless sprite.
    /// </summary>
    public Vector2I Middle { get; } = middle;

    /// <summary>
    /// Coordinates on the Atlas of the right edge of the platform.
    /// </summary>
    public Vector2I Right { get; } = right;
}
