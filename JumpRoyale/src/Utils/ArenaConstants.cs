namespace JumpRoyale.Utils;

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
}
