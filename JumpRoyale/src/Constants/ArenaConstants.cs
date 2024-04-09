namespace JumpRoyale.Utils;

/// <summary>
/// Provides a set of constants and configuration values related to drawing objects in the arena. Tweak those values in
/// order to change how the arena builder draws objects.
/// </summary>
public static class ArenaConstants
{
    /// <summary>
    /// Note: Y up goes negative, hence the sign. Modify this value if the arena has to be taller. The current 400 value
    /// defines a 6400px tall arena. There are always (n - 1) steps, so if there are e.g. three levels, there will be
    /// two sprite changes on the arena, since the first one is selected by default. If we implement more sprites per
    /// "level", e.g. 10 in total, there are 9 steps (sprite changes), so make sure this value divides nicely by the
    /// amount of difficulty levels.
    /// </summary>
    public const int MaximumArenaHeightInTiles = -400;

    /// <summary>
    /// Maximum allowed number of platforms to generate in a single row before forcing to go to the next row.
    /// </summary>
    public const int MaximumPlatformsPerRow = 3;

    /// <summary>
    /// Amount of difficulty levels. This value defines the size of components that generate values depending on the
    /// arena height processed at runtime (gradually increasing/decreasing). Some components can define certain values,
    /// like "Jumper's Scale Per Level", where LevelsCount is the arena height divisor and they can return different
    /// values every [x] height based on the current difficulty level. In general, the DifficultyLevel describes in
    /// which "block" we are right now (vertically).
    /// </summary>
    public const int DifficultyLevelsCount = 10;

    /// <summary>
    /// Arena will start drawing platforms with at least this length (excluding 2 tiles reserved for start/end).
    /// </summary>
    public const int MinBasePlatformLength = 10;

    /// <summary>
    /// Arena will start drawing platforms with at most this length.
    /// </summary>
    public const int MaxBasePlatformLength = 15;

    /// <summary>
    /// Chance to generate a new platform every column. Should be fine-tuned to generate around 2 or 3 platforms per
    /// row. Note: column = x, iterating through all columns, trying to generate a platform from that spot.
    /// </summary>
    public const float PlatformGenerationChance = 0.0125f;

    /// <summary>
    /// Chance to generate a solid block on the arena. This chance is reduced as we go up in height, reduced by certain
    /// factor every difficulty level.
    /// </summary>
    public const float BlockGenerationChance = 0.4f;

    /// <summary>
    /// Block chance generation will be reduced by this factor per each difficulty level.
    /// </summary>
    public const float BlockGenerationChanceReduction = 0.010f;

    /// <summary>
    /// Size in tiles for the full size of the separation wall. This only defines the first wall, the next walls are
    /// calculated off this value.
    /// </summary>
    public const int FullSeparationWallSize = 60;

    /// <summary>
    /// Size in tiles on Y-axis that will offset the next set of walls.
    /// </summary>
    public const int SeparationWallVerticalOffset = 10;

    /// <summary>
    /// Size in tiles for the entire Tunnel section. This includes the platforms placed at the bottom, and this value is
    /// used by the Cell Eraser to define the area.
    /// </summary>
    public const int TunnelSectionHeight = 40;

    /// <summary>
    /// Vertical spacing between each tunnel element in tiles. Ideally, this value should be the tunnel section height
    /// divisor with no remainder (height / 4 = spacing), which should be 4 times smaller, because we draw elements
    /// every [x] defined by this spacing. This is just a tweaked value to not have "unused" space afterwards.
    /// </summary>
    public const int TunnelSpacingBetweenElements = 10;

    /// <summary>
    /// Size in tiles used to create the opening at the beginning and at the end of the tunnel (floor and ceiling).
    /// </summary>
    public const int TunnelSectionHorizontalOpening = 30;
}
