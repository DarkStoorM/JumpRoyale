using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using JumpRoyale.Utils;
using JumpRoyale.Utils.Exceptions;

namespace JumpRoyale;

public partial class ArenaScene : Node2D
{
    private readonly EventTimer _lobbyCeilingTimer = new(GameConstants.LobbyAwaitingTime);

    /// <summary>
    /// Stores a dictionary of platform lengths per difficulty level.
    /// <para>
    /// "Level" changes every [x] platforms.
    /// </para>
    /// </summary>
    /// <remarks>
    /// The platforms are indexed ascending, with descending values, that way we get smaller platforms as we go up.
    /// First level gives a range of 10-15 tiles, the highest level end at 1-6.
    /// </remarks>
    private readonly Dictionary<int, int[]> _platformLengths;

    /// <summary>
    /// Similarly to <see cref="_platformLengths"/>, stores solid block size and its generation change per difficulty
    /// level.
    /// </summary>
    private readonly Dictionary<int, Tuple<int, float>> _blockSizes;

    /// <summary>
    /// Note: Y up goes negative, hence the sign. Modify this value if the arena has to be taller. The current 400 value
    /// defines a 6400px tall arena. There are always (n - 1) steps, so if there are e.g. three levels, there will be
    /// two sprite changes on the arena, since the first one is selected by default. If we implement more sprites per
    /// "level", e.g. 10 in total, there are 9 steps (sprite changes), so make sure this value divides nicely by the
    /// amount of difficulty levels.
    /// </summary>
    private readonly int _maximumArenaHeightInTiles = -400;

    /// <summary>
    /// Amount of difficulty levels.
    /// </summary>
    private readonly int _difficultyLevelsCount = 10;

    /// <summary>
    /// Maximum allowed number of platforms to generate in a single row before forcing to go to the next row.
    /// </summary>
    private readonly int _maximumPlatformsPerRow = 3;

    /// <summary>
    /// Chance to generate a new platform every column. Should be fine-tuned to generate around 2 or 3 platforms per
    /// row. Note: column = x, iterating through all columns, trying to generate a platform from that spot.
    /// </summary>
    private readonly float _chanceToGeneratePlatform = 0.0125f;

    /// <summary>
    /// Chance to generate a solid block on the arena. This chance is reduced as we go up in height, reduced by certain
    /// factor every difficulty level.
    /// </summary>
    private readonly float _chanceToGenerateBlocks = 0.4f;

    private ArenaBuilder _builder = null!;

    private Camera2D _camera = null!;

    /// <summary>
    /// Viewport Rect2.
    /// </summary>
    /// <remarks>
    /// This should not be used for Position checking. Use Camera definition instead.
    /// </remarks>
    private Rect2 _viewport;

    public ArenaScene()
    {
        // Construct difficulty values for platforms and blocks. Define more dictionaries here if needed
        _platformLengths = Enumerable
            .Range(0, _difficultyLevelsCount)
            .ToDictionary(i => i, i => new int[] { 10 - i, 15 - i });

        _blockSizes = Enumerable
            .Range(0, _difficultyLevelsCount)
            .ToDictionary(i => i, i => Tuple.Create(i, _chanceToGenerateBlocks - (0.010f * i)));

        _lobbyCeilingTimer.OnFinished += RemoveLobbyCeiling;
        _ = _lobbyCeilingTimer.Start();
    }

    public ArenaDrawingArea ArenaDrawingArea { get; private set; } = null!;

    /// <summary>
    /// Scaled viewport size. Assuming the viewport was set to always be of the same size despite the window size.
    /// </summary>
    public Vector2 ViewportSizeInPixels => _viewport.Size;

    /// <summary>
    /// Describes the visible TileMap portion of the viewport in tiles.
    /// </summary>
    public Vector2I ViewportSizeInTiles => new((int)_viewport.Size.X / 16, (int)_viewport.Size.Y / 16);

    public override void _Ready()
    {
        _camera = GetNode<Camera2D>("CameraScene/CameraNode");
        TileMap tileMap = GetNode<TileMap>("TileMap");

        if (tileMap.TileSet is null)
        {
            throw new UnassignedSceneOrComponentException();
        }

        _builder = new ArenaBuilder(tileMap, _maximumArenaHeightInTiles / 3);
        _viewport = GetViewportRect();

        // Construct the "playable" arena field from the viewport
        // Note: node has to enter the scene first, so we can't use this in the constructor
        ArenaDrawingArea = new()
        {
            StartX = 2,
            EndX = ViewportSizeInTiles.X - 2,
            SizeInTiles = ViewportSizeInTiles.X - 4,
        };

        GenerateArena();
        DrawLobbyCeiling();
    }

    /// <summary>
    /// Pre-generates the entire arena, column-by-column, row-by-row, including extra blocks and different platform/wall
    /// shapes. Platforms will always be drawn in a way so they leave one tile next to the wall (left and right side).
    /// </summary>
    /// <remarks>
    /// Platforms are generated every second row, because they are too close to each other if we generate them every
    /// row.
    /// </remarks>
    private void GenerateArena()
    {
        // Note: this method has to generate objects in separate loops, because if we do this in the same loop, bigger
        // objects will be overwritten, because we don't perform any existence checks. It is better if the objects
        // overwrite others separately to prevent "drilling" ugly holes in the objects. This only applies to platforms,
        // because they are smaller, so if they overwrite anything, the spacing just looks bad. We could do this with a
        // different TileMap, but whatever.
        DrawSideWalls();

        int startY = 0;
        int endY = _maximumArenaHeightInTiles;

        // Randomly draw the platforms as base grounding
        for (int y = startY; y > endY; y -= 2)
        {
            GeneratePlatformAtY(y);
        }

        // Overlay blocks over platforms, one per row
        for (int y = startY; y > endY; y -= 2)
        {
            GenerateBlockAtY(y);
        }

        // Create a set of separation walls of a fixed height, but only once, at certain height
        int stoppedDrawingAtY = GenerateSeparationWalls();

        // Create a tunnel, but only once at random Y __above separation walls__
        GenerateTunnel(stoppedDrawingAtY);
    }

    private void GeneratePlatformAtY(int y)
    {
        int platformLength = GetNextPlatformLength(y);

        // Draw on the playable arena field, excluding the current platform length to prevent from drawing off
        // screen
        int startingColumn = ArenaDrawingArea.StartX;
        int endingColumn = ArenaDrawingArea.EndX - platformLength - 2;

        int platformsGeneratedThisRow = 0;
        int currentColumn = startingColumn - 1;

        // Loop through the range of allowed columns on X-axis
        while (currentColumn < endingColumn)
        {
            currentColumn++;

            // Go to the next column and try to generate a new platform
            if (Rng.RandomFloat() > _chanceToGeneratePlatform)
            {
                continue;
            }

            _builder.DrawHorizontalPlatform(new(currentColumn, y), platformLength);
            platformsGeneratedThisRow++;

            // Skip as many columns as it took to draw the platform + offset (1 space + platform edges: L/R)
            // This way we don't draw over the previous platform
            currentColumn += platformLength + 3;

            // Force skipping to the next row if we already generated enough platforms in the current row
            if (platformsGeneratedThisRow == _maximumPlatformsPerRow)
            {
                break;
            }
        }
    }

    /// <summary>
    /// Draws a square block on the arena at given Y on successful generation chance roll. The X component is chosen at
    /// random, generating only one block per row.
    /// </summary>
    /// <param name="y">Current arena height.</param>
    private void GenerateBlockAtY(int y)
    {
        (int blockSize, float chanceToGenerate) = GetNextBlockData(y);

        // Skip this row and try to generate a new block
        if (Rng.RandomFloat() > chanceToGenerate)
        {
            return;
        }

        int x = Rng.IntRange(ArenaDrawingArea.StartX, ArenaDrawingArea.EndX - blockSize - 2);

        _builder.DrawSquare(new(x, y), blockSize, shouldFill: true, fillWith: TileTypes.Stone);
    }

    /// <summary>
    /// Draws vertical walls in steps: 1 -> 2 -> 3, with the main wall being the tallest, then two sets of smaller walls
    /// (50% size). This will return the Y position where the walls were finished drawing + offset.
    /// </summary>
    private int GenerateSeparationWalls()
    {
        // Generate the wall only at certain point, somewhere around the middle of the arena + 2 smaller walls, then 3
        int centerPoint = _maximumArenaHeightInTiles / 2;
        int randomY = Rng.IntRange((int)(centerPoint / 1.5), centerPoint / 2);
        int fullWallSize = 60;
        int smallWall = fullWallSize / 2;
        int wallsVerticalOffset = 10;

        // Marker at the quarter of the arena for easier positioning
        int quarterArenaSize = ArenaDrawingArea.SizeInTiles / 4;

        // Draw the main wall in the middle of the arena
        _builder.DrawVerticalWall(new(quarterArenaSize * 2, randomY), fullWallSize);

        // Offset the next walls by the first wall length and with some breathing room
        int nextWallYPosition = randomY - fullWallSize - wallsVerticalOffset;

        // Two smaller walls above the main wall
        _builder.DrawVerticalWall(new(quarterArenaSize, nextWallYPosition), smallWall);
        _builder.DrawVerticalWall(new(quarterArenaSize * 3, nextWallYPosition), smallWall);

        // Offset the next walls + some breathing room again
        nextWallYPosition -= smallWall + wallsVerticalOffset;

        // Three smaller walls above the secondary, smaller walls dividing the arena into 4 parts: middle + at the
        // center of the separated parts
        _builder.DrawVerticalWall(new(quarterArenaSize / 2, nextWallYPosition), smallWall);
        _builder.DrawVerticalWall(new(quarterArenaSize * 2, nextWallYPosition), smallWall);
        _builder.DrawVerticalWall(new((int)(quarterArenaSize * 3.5), nextWallYPosition), smallWall);

        // Return the position above the walls, because we actually want them to be generated and we don't want them to
        // be overwritten by the tunnel
        return nextWallYPosition - smallWall;
    }

    /// <summary>
    /// Generates an obstacle in a horizontal tunnel shape, blocking the path.
    /// </summary>
    private void GenerateTunnel(int previousY)
    {
        // Rather than picking a random Y where we will generate the tunnel, we will advance up from where the walls
        // were previously generated (inserting it above the walls), and only then add an offset, roughly one screen at
        // most
        int randomY = Rng.IntRange(previousY - 70, previousY);
        int arenaWidth = ArenaDrawingArea.SizeInTiles;

        // Height of the entire tunnel part, including the safe platforms below it
        int tunnelHeight = 40;

        // Gap in the floor and ceiling (entrance/exit)
        int tunnelOpening = 30;

        // Erase the entire area, where the tunnel will be inserted
        _builder.EraseSpritesAtArea(
            new(ArenaDrawingArea.StartX - 1, randomY),
            new(arenaWidth + 2, randomY - tunnelHeight)
        );

        // Draw the safe floor first, this will fill the entire width. Due to how the blocks are generated, we will
        // actually need two platforms
        _builder.DrawHorizontalPlatform(new(1, randomY), arenaWidth);
        _builder.DrawHorizontalPlatform(new(1, randomY - 10), arenaWidth);

        // Make openings at random, left or right
        bool startsFromLeft = Rng.RandomBool();
        int drawFloorAtX = startsFromLeft ? 1 : tunnelOpening;
        int drawCeilingAtX = startsFromLeft ? tunnelOpening : 1;
        int wallLength = arenaWidth - tunnelOpening + 1;
        int drawPlatformAtX = startsFromLeft ? 1 : wallLength;

        // Draw the tunnel floor and ceiling with openings on the opposite sides
        _builder.DrawHorizontalWall(new(drawFloorAtX, randomY - 20), wallLength);
        _builder.DrawHorizontalWall(new(drawCeilingAtX, randomY - 40), wallLength);

        // Draw helper "escape platform" below the ceiling to allow escaping
        _builder.DrawHorizontalPlatform(new(drawPlatformAtX, randomY - 30), tunnelOpening);

        // Note: we could generate a small block inside the tunnel, but no idea how difficult it would be to navigate
        // through it for the players, but maybe just a tiny 1-tile cell in the middle would also be fine
    }

    /// <summary>
    /// Returns the platform length for the current difficulty level.
    /// </summary>
    /// <param name="currentY">Current arena height.</param>
    private int GetNextPlatformLength(int currentY)
    {
        int index = GetDifficultyLevelFromHeight(currentY);

        int[] lengths = _platformLengths[index];

        return Rng.IntRange(lengths[0], lengths[1]);
    }

    /// <summary>
    /// Returns the block size and its chance to be generated based on the current "Level", which is defined by how high
    /// on the arena we are.
    /// </summary>
    /// <param name="currentY">Current arena height.</param>
    private Tuple<int, float> GetNextBlockData(int currentY)
    {
        int index = GetDifficultyLevelFromHeight(currentY);

        (int blockSize, float chanceToGenerate) = _blockSizes[index];

        return new(blockSize, chanceToGenerate);
    }

    /// <summary>
    /// Calculates the dictionary index for platform lengths and block sizes based on how many "steps" we fit into the
    /// current Y, where "step" is the amount of tiles required to switch to the higher level on Y-axis.
    /// <para>Example:</para>
    /// <para>- Maximum height is 600 and we predefined 10 levels = Step is 60 (level up every 60 tiles).</para>
    /// <para>- Current Y is 240, the index evaluates to 4 (fifth index from 0).</para>
    /// </summary>
    /// <param name="currentY">Current arena height.</param>
    private int GetDifficultyLevelFromHeight(int currentY)
    {
        // Note: Y is negative.
        int index = Math.Abs(currentY / (_maximumArenaHeightInTiles / _blockSizes.Count));

        // Prevent index overflow for components that depend on the difficulty level, e.g. platform length per level
        index = Math.Clamp(index, 0, _difficultyLevelsCount - 1);

        return index;
    }

    /// <summary>
    /// Draws arena boundaries - left and right walls up to the defined maximum height.
    /// </summary>
    private void DrawSideWalls()
    {
        // The very bottom of the stage (right above the ground)
        int startingYPoint = 0;

        // Wall height to draw per "step"
        int wallHeightPerStep = Math.Abs(_maximumArenaHeightInTiles / 3);

        // We need to manually define the sprite differences on given height, since we are not using the same sprite to
        // draw everything, only the generated platform will change automatically.
        for (int i = 0; i < 3; i++)
        {
            // Left wall
            _builder.DrawVerticalWall(
                new(0, startingYPoint - (wallHeightPerStep * i)),
                wallHeightPerStep - 2,
                _builder.TileTypeByIndex(i)
            );

            // Right wall
            // Note: Viewport is 120 tiles long, but we have to draw on the last column, so -1, otherwise we end up off
            // screen
            _builder.DrawVerticalWall(
                new(ViewportSizeInTiles.X - 1, startingYPoint - (wallHeightPerStep * i)),
                wallHeightPerStep - 2,
                _builder.TileTypeByIndex(i)
            );
        }
    }

    /// <summary>
    /// Draws a horizontal wall across the entire arena, blocking the passage until the lobby timer is done counting.
    /// </summary>
    private void DrawLobbyCeiling()
    {
        _builder.DrawHorizontalWall(
            new(ArenaDrawingArea.StartX - 1, -18),
            ArenaDrawingArea.SizeInTiles,
            TileTypes.Gold
        );
    }

    /// <summary>
    /// Removes the blocking horizontal wall, releasing the players after the lobby timer countdown and starts the
    /// camera scroll.
    /// </summary>
    private void RemoveLobbyCeiling(object sender, EventArgs args)
    {
        _builder.EraseSpritesAtArea(new(ArenaDrawingArea.StartX - 1, -18), new(ArenaDrawingArea.SizeInTiles + 2, -18));
    }
}
