using Godot;

namespace JumpRoyale;

public abstract class BaseLineObject(Vector2I start, Vector2I middle, Vector2I end) : IBaseLineObject
{
    public Vector2I Start { get; } = start;

    public Vector2I Middle { get; } = middle;

    public Vector2I Finish { get; } = end;
}
