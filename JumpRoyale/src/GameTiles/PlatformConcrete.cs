using Godot;

namespace JumpRoyale;

public class PlatformConcrete(Vector2I left, Vector2I middle, Vector2I right)
    : BaseHorizontalObject(left, middle, right) { }
