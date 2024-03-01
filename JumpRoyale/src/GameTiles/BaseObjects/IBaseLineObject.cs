using Godot;

namespace JumpRoyale;

public interface IBaseLineObject
{
    /// <summary>
    /// Coordinates on the Atlas of the starting edge (or top) of the object. This object is always drawn when user
    /// requests to draw a line.
    /// </summary>
    Vector2I Start { get; }

    /// <summary>
    /// Coordinates on the Atlas of the middle part of the object. This part is used to draw the "extension" of a
    /// drawn object. This should represent a seamless sprite that connects to both starting and ending sides and can
    /// repeat on either side.
    /// </summary>
    Vector2I Middle { get; }

    /// <summary>
    /// Coordinates on the Atlas of the ending edge (or bottom) of the object. This object is always drawn when user
    /// requests to draw a line.
    /// </summary>
    Vector2I Finish { get; }
}
