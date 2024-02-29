using Godot;

namespace JumpRoyale;

public abstract class BaseVerticalObject(Vector2I top, Vector2I middle, Vector2I bottom)
{
    /// <summary>
    /// Coordinates on the Atlas of the top edge of the object.
    /// </summary>
    public Vector2I Top { get; } = top;

    /// <summary>
    /// Coordinates on the Atlas of the middle part of the object. This part is used to draw the "extension" of a
    /// drawn object. This should represent a seamless sprite that connects to both top and bottom sides.
    /// </summary>
    public Vector2I Middle { get; } = middle;

    /// <summary>
    /// Coordinates on the Atlas of the bottom edge of the object.
    /// </summary>
    public Vector2I Bottom { get; } = bottom;
}
