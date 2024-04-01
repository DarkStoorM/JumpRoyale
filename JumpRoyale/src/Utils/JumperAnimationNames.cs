using JumpRoyale.Utils.Extensions;

namespace JumpRoyale.Utils;

/// <summary>
/// Provides converted names of animation Enums.
/// </summary>
public static class JumperAnimationNames
{
    public static string Fall => JumperAnimation.FALL.GetValue();

    public static string Idle => JumperAnimation.IDLE.GetValue();

    public static string Jump => JumperAnimation.JUMP.GetValue();

    public static string Land => JumperAnimation.LAND.GetValue();
}
