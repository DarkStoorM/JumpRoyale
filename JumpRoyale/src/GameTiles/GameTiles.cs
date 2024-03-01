namespace JumpRoyale;

/// <summary>
/// Provides a set of object instances for drawing on the tilemap.
/// </summary>
public static class GameTiles
{
    public static readonly BlockConcrete BlockConcrete = new(new(12, 2));
    public static readonly BlockGold BlockGold = new(new(17, 9));
    public static readonly BlockStone BlockStone = new(new(12, 1));

    public static readonly PlatformConcrete PlatformConcrete = new(new(17, 2), new(18, 2), new(19, 2));
    public static readonly PlatformGold PlatformGold = new(new(17, 0), new(18, 0), new(19, 0));
    public static readonly PlatformStone PlatformStone = new(new(17, 1), new(18, 1), new(19, 1));

    public static readonly VerticalWallConcrete VerticalWallConcrete = new(new(15, 4), new(15, 5), new(15, 6));
    public static readonly VerticalWallGold VerticalWallGold = new(new(20, 8), new(20, 9), new(20, 10));
    public static readonly VerticalWallStone VerticalWallStone = new(new(15, 0), new(15, 1), new(15, 2));

    public static readonly HorizontalWallConcrete HorizontalWallConcrete = new(new(12, 4), new(13, 4), new(14, 4));
    public static readonly HorizontalWallGold HorizontalWallGold = new(new(17, 8), new(18, 8), new(19, 8));
    public static readonly HorizontalWallStone HorizontalWallStone = new(new(12, 0), new(13, 0), new(14, 0));
}
