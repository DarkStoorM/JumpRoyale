using Godot;

namespace JumpRoyale;

public interface IBasePointObject
{
    /// <summary>
    /// Coordinates on the Atlas for the sprite of this block. This should represent a single block, which does not
    /// connect to any other sprites. This does not mean it should not be tiled, though, it still can be used as a tiled
    /// object if there are no dedicated sprites for that object.
    /// </summary>
    public Vector2I SpriteLocation { get; }
}
