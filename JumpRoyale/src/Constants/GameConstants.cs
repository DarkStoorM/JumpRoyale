using Godot;

namespace JumpRoyale;

/// <summary>
/// Class providing constants serving as values for all game components.
/// </summary>
public static class GameConstants
{
    public const int TileSizeInPixels = 16;

    /// <summary>
    /// Time in seconds before the game starts. While lobby timer is active, the camera will also be stopped for in this
    /// period.
    /// </summary>
    public const int LobbyAwaitingTime = 40;

    /// <summary>
    /// Game time in seconds after the awaiting time (lobby countdown).
    /// </summary>
    public const int GameTime = 150;

    /// <summary>
    /// Time in seconds defining for how long the players will not be allowed to jump after the game is finished.
    /// </summary>
    public const int PodiumTimeout = 5;

    /// <summary>
    /// Interval in seconds, the scroll speed will increase at this interval (Camera's movement speed).
    /// </summary>
    public const int ScrollSpeedChangeInterval = 15;

    /// <summary>
    /// Base speed multiplied by increasing factor over time. Describes the initial Delta multiplier.
    /// </summary>
    public const int BaseCameraMovementSpeed = 10;

    public const int ScreenWidthInPixels = 1920;

    public const int ScreenHeightInPixels = 1072;

    public static readonly float Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
}
