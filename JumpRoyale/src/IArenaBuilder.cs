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
    /// <param name="drawWith">Type of the platform to draw. This parameter does not have to be specified in order to
    /// draw with the default <c>Stone</c> tile. Override this if you need to draw with a different tile.</param>
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
    /// <param name="drawWith">Type of the wall to draw. This parameter does not have to be specified in order to
    /// draw with the default <c>Stone</c> tile. Override this if you need to draw with a different tile.</param>
    public void DrawHorizontalWall(Vector2I location, int length, TileTypes drawWith = TileTypes.Stone);

    /// <summary>
    /// Draws a vertical wall on the TileMap in specified location. vertical walls represent a 3-tile sprite
    /// combination, which consist of two edges and optional "middle" part, which is dictated by the <c>height</c>, so
    /// if no height is provided, it will be drawn as a 2-tile sprite.
    /// <para>
    /// This draws a solid wall the player can't jump through, drawn bottom-to-top.
    /// </para>
    /// </summary>
    /// <remarks>
    /// The shortest wall is 2 tiles tall, which always have Top and Bottom parts. <c>height</c> parameter specifies
    /// how many additional tiles to draw, which extend the wall, so despite providing the height of 0, the wall
    /// will always be drawn.
    /// </remarks>
    /// <param name="location">Starting point where the wall is drawn (starting from Bottom).</param>
    /// <param name="height">How many Middle tiles to insert.</param>
    /// <param name="drawWith">Type of the wall to draw. This parameter does not have to be specified in order to
    /// draw with the default <c>Stone</c> tile. Override this if you need to draw with a different tile.</param>
    public void DrawVerticalWall(Vector2I location, int height, TileTypes drawWith = TileTypes.Stone);

    /// <summary>
    /// Draws a single sprite on the TileMap at given location.
    /// </summary>
    /// <param name="location">Coordinates on TileMap to draw at.</param>
    /// <param name="drawWith">Type of the wall to draw. This parameter does not have to be specified in order to
    /// draw with the default <c>Stone</c> tile. Override this if you need to draw with a different tile.</param>
    public void DrawPoint(Vector2I location, TileTypes drawWith = TileTypes.Stone);

    /// <summary>
    /// Draws a square-shaped object on the arena at the specified location. By default, a square of 1 tile will be
    /// drawn and if the <c>size</c> is specified (>0), the object will be extended diagonally up-right by this many
    /// units.
    /// </summary>
    /// <remarks>
    /// The starting point of the square is bottom-left. The object is drawn from left-to-right, bottom-to-top.
    /// </remarks>
    /// <param name="location">Starting point where the object is drawn from.</param>
    /// <param name="size">Amount of units this square will be expanded diagonally for.</param>
    /// <param name="shouldFill">If true, the object will be filled with sprites (default or manually selected).</param>
    /// <param name="drawWith">Primary object to draw the bounds with. This parameter does not have to be specified in
    /// order to draw with the default <c>Stone</c> tile. Override this if you need to draw with a different
    /// tile.</param>
    /// <param name="fillWith">Secondary object used as a filler. <c>drawWith</c> is used as default if none was
    /// specified and if <c>shouldFill</c> was true.</param>
    public void DrawSquare(
        Vector2I location,
        int size,
        bool shouldFill = false,
        TileTypes drawWith = TileTypes.Stone,
        TileTypes? fillWith = null
    );

    /// <summary>
    /// Draws a rectangle object, whose size is defined by the <c>endingPoint</c>. By default, a rectangle of 1 tile
    /// will be drawn and if the <c>size</c> is specified (>0).
    /// </summary>
    /// <remarks>
    /// The drawing method is as same as for Square, bottom-left -> top-right.
    /// </remarks>
    /// <param name="startingPoint">Location of the bottom-left corner to start drawing from.</param>
    /// <param name="endingPoint">Location of the top-right corner to end the drawing at.</param>
    /// <param name="shouldFill">If true, the object will be filled with sprites (default or manually selected).</param>
    /// <param name="drawWith">Primary object to draw the bounds with. This parameter does not have to be specified in
    /// order to draw with the default <c>Stone</c> tile. Override this if you need to draw with a different
    /// tile.</param>
    /// <param name="fillWith">Secondary object used as a filler. <c>drawWith</c> is used as default if none was
    /// specified and if <c>shouldFill</c> was true.</param>
    public void DrawBox(
        Vector2I startingPoint,
        Vector2I endingPoint,
        bool shouldFill = false,
        TileTypes drawWith = TileTypes.Stone,
        TileTypes? fillWith = null
    );
}
