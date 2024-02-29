namespace JumpRoyale;

/// <summary>
/// Provides a set of object instances for drawing on the tilemap.
/// </summary>
public static class GameTiles
{
    /// <summary>
    /// One-way collision platform (concrete-like sprites).
    /// </summary>
    public static readonly PlatformConcrete PlatformConcrete = new(new(17, 2), new(18, 2), new(19, 2));

    /// <summary>
    /// One-way collision platform (gold-like sprites).
    /// </summary>
    public static readonly PlatformGold PlatformGold = new(new(17, 0), new(18, 0), new(19, 0));

    /// <summary>
    /// One-way collision platform (stone-like sprites). This platform is used as default.
    /// </summary>
    public static readonly PlatformStone PlatformStone = new(new(17, 1), new(18, 1), new(19, 1));
}
