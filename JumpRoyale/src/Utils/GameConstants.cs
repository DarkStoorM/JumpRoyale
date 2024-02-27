using Godot;

namespace JumpRoyale;

public static class GameConstants
{
    public static readonly float Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
}
