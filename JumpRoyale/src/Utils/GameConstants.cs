using Godot;

namespace JumpRoyale;

/// <summary>
/// Class providing constants serving as values for all game components.
/// </summary>
public static class GameConstants
{
    public static readonly float Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
}
