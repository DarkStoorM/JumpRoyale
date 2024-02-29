using Godot;

namespace JumpRoyale;

public abstract class BaseHorizontalObject(Vector2I left, Vector2I middle, Vector2I right)
{
    /// <summary>
    /// Coordinates on the Atlas of the left edge of the object.
    /// </summary>
    public Vector2I Left { get; } = left;

    /// <summary>
    /// Coordinates on the Atlas of the middle part of the object. This part is used to draw the "extension" of a
    /// drawn object. This should represent a seamless sprite that connects to both left and right sides.
    /// </summary>
    public Vector2I Middle { get; } = middle;

    /// <summary>
    /// Coordinates on the Atlas of the right edge of the object.
    /// </summary>
    public Vector2I Right { get; } = right;
}
