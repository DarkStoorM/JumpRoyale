using Godot;

namespace JumpRoyale;

/// <summary>
/// Class providing constants serving as values for all game components.
/// </summary>
public static class GameConstants
{
    public static readonly float Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    /// <summary>
    /// Time before the game starts. While lobby timer is active, the camera will also be stopped for in this period.
    /// </summary>
    public static readonly int LobbyAwaitingTimeInSeconds = 40;

    /// <summary>
    /// Game time after the awaiting time (lobby countdown).
    /// </summary>
    public static readonly int GameTimeInSeconds = 150;

    /// <summary>
    /// Interval in seconds, the scroll speed will increase at this interval (Camera's movement speed).
    /// </summary>
    public static readonly int ScrollSpeedChangeInterval = 15;

    /// <summary>
    /// Base speed multiplied by increasing factor over time. Describes the initial Delta multiplier.
    /// </summary>
    public static readonly int BaseCameraMovementSpeed = 10;
}
