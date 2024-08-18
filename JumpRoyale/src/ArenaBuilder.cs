using Godot;

public class ArenaBuilder(TileMap tileMap)
{
    /// <summary>
    /// Defines at what Y in tiles the object type should change. For reference, the screen is 63 tiles tall on a 1080p
    /// screen and Y goes negative upwards. Important note: the current arena is placed one screen below the default
    /// position, so the actual next screen is located at Y=0. The estimate average max height the players reach
    /// during the game is around 3500px (actually -155 height in the game), so the last texture change should be
    /// "almost" at the estimated max height. Despite the screen being 63 tiles tall, we change the object type every 50
    /// tiles to give players a chance to see all the textures. With 63~ tiles per change, the last change occurs at
    /// 4000px, which will barely be seen by players.
    /// </summary>
    private readonly int[] _objectTypeChangeHeights = [0, -50, -100, -150];
    private readonly TileMap _tileMap = tileMap;

    public void DrawPlatform(int x, int y, int width)
    {
        BaseObject gameObject = GameObject.Get(GetObjectType(y));
        int endX = x + width - 1;

        // Draw left side of the platform
        DrawAt(x, y, gameObject.Left);

        // Draw middle if width > 2
        if (width > 2)
        {
            for (int i = x + 1; i < endX; i++)
            {
                DrawAt(i, y, gameObject.Middle);
            }
        }

        // Draw right side of the platform
        DrawAt(endX, y, gameObject.Right);
    }

    public void DrawPoint(int x, int y)
    {
        BaseObject gameObject = GameObject.Get(GetObjectType(y));

        DrawAt(x, y, gameObject.Point);
    }

    public void RemovePoint(int x, int y) => RemoveAt(x, y);

    private GameObjectType GetObjectType(int y)
    {
        return y switch
        {
            // Note: can't use a simple "< _objectTypeChangeHeights[3]", because it requires a constant value
            var value when value < _objectTypeChangeHeights[3] => GameObjectType.Gold,
            var value when value < _objectTypeChangeHeights[2] => GameObjectType.Brick,
            var value when value < _objectTypeChangeHeights[1] => GameObjectType.Terracotta,
            var value when value < _objectTypeChangeHeights[0] => GameObjectType.Concrete,
            _ => GameObjectType.Stone // Default object will always be drawn first - the brown stone object
        };
    }

    private void DrawAt(int x, int y, Vector2I obj)
    {
        _tileMap.SetCell(0, new Vector2I(x, y), 0, obj);
    }

    private void RemoveAt(int x, int y)
    {
        _tileMap.SetCell(0, new Vector2I(x, y), -1);
    }
}
