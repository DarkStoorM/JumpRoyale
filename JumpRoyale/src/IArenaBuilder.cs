using Godot;

namespace JumpRoyale;

public interface IArenaBuilder
{
    /// <summary>
    /// Draws a horizontal platform on the TileMap in specified location. Horizontal objects represent a 3-tile sprite
    /// combination, which consist of two edges and optional "middle" part, which is dictated by the <c>length</c>, so
    /// if no length is provided, it will be drawn as a 2-tile sprite.
    /// <para>
    /// This draws a one-way collision platform, drawn left-to-right.
    /// </para>
    /// </summary>
    /// <remarks>
    /// The shortest platform is 2 tiles long, which always have Left and Right parts. <c>length</c> parameter specifies
    /// how many additional tiles to draw, which extend the platform, so despite providing the length of 0, the platform
    /// will always be drawn.
    /// </remarks>
    /// <param name="location">Starting point where the platform is drawn (starting from Left).</param>
    /// <param name="length">How many Middle tiles to insert.</param>
    /// <param name="drawWith">Type of the platform to draw.</param>
    public void DrawHorizontalPlatform(Vector2I location, int length, TileTypes drawWith = TileTypes.Stone);

    /// <summary>
    /// Draws a horizontal wall on the TileMap in specified location. Horizontal walls represent a 3-tile sprite
    /// combination, which consist of two edges and optional "middle" part, which is dictated by the <c>length</c>, so
    /// if no length is provided, it will be drawn as a 2-tile sprite.
    /// <para>
    /// This draws a solid wall the player can't jump through, drawn left-to-right.
    /// </para>
    /// </summary>
    /// <remarks>
    /// The shortest wall is 2 tiles long, which always have Left and Right parts. <c>length</c> parameter specifies
    /// how many additional tiles to draw, which extend the wall, so despite providing the length of 0, the wall
    /// will always be drawn.
    /// </remarks>
    /// <param name="location">Starting point where the wall is drawn (starting from Left).</param>
    /// <param name="length">How many Middle tiles to insert.</param>
    /// <param name="drawWith">Type of the wall to draw.</param>
    public void DrawHorizontalWall(Vector2I location, int length, TileTypes drawWith = TileTypes.Stone);

    /// <summary>
    /// Draws a vertical wall on the TileMap in specified location. vertical walls represent a 3-tile sprite
    /// combination, which consist of two edges and optional "middle" part, which is dictated by the <c>height</c>, so
    /// if no height is provided, it will be drawn as a 2-tile sprite.
    /// <para>
    /// This draws a solid wall the player can't jump through, drawn top-to-bottom.
    /// </para>
    /// </summary>
    /// <remarks>
    /// The shortest wall is 2 tiles tall, which always have Top and Bottom parts. <c>height</c> parameter specifies
    /// how many additional tiles to draw, which extend the wall, so despite providing the height of 0, the wall
    /// will always be drawn.
    /// </remarks>
    /// <param name="location">Starting point where the wall is drawn (starting from Top).</param>
    /// <param name="height">How many Middle tiles to insert.</param>
    /// <param name="drawWith">Type of the wall to draw.</param>
    public void DrawVerticalWall(Vector2I location, int height, TileTypes drawWith = TileTypes.Stone);

    /// <summary>
    /// Draws a single sprite on the TileMap at given location.
    /// </summary>
    /// <param name="location">Coordinates on TileMap to draw at.</param>
    public void DrawPoint(Vector2I location, TileTypes drawWith = TileTypes.Stone);
}
