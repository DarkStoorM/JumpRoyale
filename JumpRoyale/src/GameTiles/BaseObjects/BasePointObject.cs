using Godot;

namespace JumpRoyale;

public abstract class BasePointObject(Vector2I spriteLocation)
{
    public Vector2I SpriteLocation { get; } = spriteLocation;
}
