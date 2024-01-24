using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Godot;
using TwitchChat;
using TwitchLib.PubSub.Events;

public partial class Arena : Node2D
{
    private const int WallHeightInTiles = 15;
    private const int ArenaHeightInTiles = 600;

    private const string LobbyOverlayNodeName = "LobbyOverlay";
    private const string GameOverlayNodeName = "GameOverlay";
    private const string EndScreenOverlayNodeName = "EndScreenOverlay";
    private const string CameraNodeName = "Camera";
    private const string CanvasLayerNodeName = "CanvasLayer";
    private const string SaveLocation = "res://save_data/players.json";

    private readonly Dictionary<string, Jumper> _jumpers = new();
    private AllPlayerData _allPlayerData = new();
    private TileMap _lobbyTilemap = new();
    private RandomNumberGenerator _rng = new();

    [Export]
    private PackedScene? _jumperScene;

    [Export]
    private TileSet? _tileSetToUse;

    private bool _hasGameEnded;

    private int _ceilingHeight;
    private int _choice = 1;
    private int _generatedMaxHeight;
    private int _heightInTiles;
    private int _widthInTiles;

    private long _timeSinceGameEnd;

    [Signal]
    public delegate void PlayerCountChangeEventHandler(int numPlayers);

    [Signal]
    public delegate void MaxHeightChangedEventHandler(string playerName, int height);

    [Signal]
    public delegate void CameraSpeedChangedEventHandler(int speed);

    public override void _Ready()
    {
        _lobbyTilemap = new TileMap { Name = "TileMap", TileSet = _tileSetToUse };

        TwitchChatClient twitchChatClient = new();

        twitchChatClient.OnRedemption += OnRedemption;
        twitchChatClient.OnMessage += OnMessage;
        GetLobbyOverlay().TimerDone += OnLobbyTimerDone;
        GetGameOverlay().TimerDone += OnGameTimerDone;

        SetBackground();
        GenerateLobby();
        LoadPlayerData();
    }

    public override void _PhysicsProcess(double delta)
    {
        ModifyPlayerScales();
        GenerateProceduralPlatforms();
        MoveCamera();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_accept") && Input.IsPhysicalKeyPressed(Key.Alt))
        {
            DisplayServer.WindowSetMode(
                DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen
                    ? DisplayServer.WindowMode.Maximized
                    : DisplayServer.WindowMode.Fullscreen
            );
        }

        if (Input.IsPhysicalKeyPressed(Key.W) && Input.IsPhysicalKeyPressed(Key.Ctrl))
        {
            // Note: calling this a few times when at least one player is on the screen will cause the game to
            // crash, investigate what is causing the crash here. Low priority anyway, because this is only
            // a streamer tool for quick debugging purposes
            OnGameTimerDone();
        }
    }

    private void OnMessage(object sender, MessageEventArgs e)
    {
        // This is kind of a workaround for now to have a top level Defer and be able to pass objects around inside,
        // which can't be converted to Godot's Variant. We still have to pull the required data separately, because `e`
        // is an object :(
        CallDeferred(nameof(HandleCommands), e.Message, e.SenderId, e.SenderName, e.HexColor, e.IsPrivileged);
    }

    private void HandleCommands(string message, string senderId, string senderName, string hexColor, bool isPrivileged)
    {
        ChatCommandParser command = new(message.ToLower());

        string?[] stringArguments = command.ArgumentsAsStrings();
        int?[] numericArguments = command.ArgumentsAsNumbers();

        // Join is the only command that can be executed by everyone, whether joined or not.
        // All the remaining commands are only available to those who joined the game
        if (CommandMatcher.MatchesJoin(command.Name))
        {
            HandleJoin(senderId, senderName, hexColor, isPrivileged);
            return;
        }

        if (!_jumpers.TryGetValue(senderId, out Jumper? jumper))
        {
            return;
        }

        // Important: when working with aliases that collide with each other, remember to use the
        // proper order. E.g. Jump has `u` alias and if it was first on the list, it would
        // execute if `unglow` was sent in the chat, because we don't use exact matching
        switch (command.Name)
        {
            // -- Commands for all Chatters (active)
            case string when CommandMatcher.MatchesUnglow(command.Name):
                HandleUnglow(jumper);
                break;

            case string when CommandMatcher.MatchesJump(command.Name):
                HandleJump(jumper, command.Name, numericArguments[0], numericArguments[1]);
                break;

            case string when CommandMatcher.MatchesCharacterChange(command.Name):
                HandleCharacterChange(jumper, numericArguments[0]);
                break;

            // -- Commands for Mods, VIPs, Subs
            case string when CommandMatcher.MatchesGlow(command.Name, isPrivileged):
                HandleGlow(jumper, stringArguments[0], hexColor);
                break;
        }
    }

    private void HandleJoin(string userId, string userName, string hexColor, bool isPrivileged)
    {
        Ensure.IsNotNull(_jumperScene);
        Ensure.IsNotNull(_tileSetToUse);

        if (_jumpers.ContainsKey(userId))
        {
            return;
        }

        int randomCharacterChoice = _rng.RandiRange(1, 18);

        PlayerData playerData = _allPlayerData.Players.ContainsKey(userId)
            ? _allPlayerData.Players[userId]
            : new PlayerData(hexColor, randomCharacterChoice);

        _allPlayerData.Players[userId] = playerData;

        // Even if the player already existed, we may need to update their name.
        playerData.Name = userName;
        playerData.UserId = userId;

        Jumper jumper = (Jumper)_jumperScene.Instantiate();
        Rect2 viewport = GetViewportRect();
        int tileHeight = _tileSetToUse.TileSize.Y;
        int xPadding = _tileSetToUse.TileSize.X * 3;
        int x = _rng.RandiRange(xPadding, (int)viewport.Size.X - xPadding);
        int y = ((int)(viewport.Size.Y / tileHeight) - 1 - WallHeightInTiles) * tileHeight;

        jumper.Init(x, y, userName, playerData);
        AddChild(jumper);

        // Note: the following block requires the jumper to be initialized before performing any changes,
        // either cosmetic or on the jumper himself, because we have to read the input from playerData,
        // which has to be sent to the jumper through .Init() first.
        jumper.SetCharacter(playerData.CharacterChoice);

        if (!isPrivileged)
        {
            jumper.DisableGlow();
        }

        _jumpers.Add(userId, jumper);

        EmitSignal(SignalName.PlayerCountChange, _jumpers.Count);
    }

    private void HandleGlow(Jumper jumper, string? userHexColor, string twitchChatHexColor)
    {
        string glowColor = userHexColor is not null ? userHexColor : twitchChatHexColor;

        jumper.SetGlow(glowColor);
    }

    private void HandleUnglow(Jumper jumper)
    {
        jumper.DisableGlow();
    }

    private void HandleCharacterChange(Jumper jumper, int? userChoice)
    {
        int choice = userChoice ?? _rng.RandiRange(1, 18);

        choice = Math.Clamp(choice, 1, 18);

        jumper.SetCharacter(choice);
    }

    private void HandleJump(Jumper jumper, string direction, int? angle, int? jumpPower)
    {
        if (!IsAllowedToJump())
        {
            return;
        }

        JumpCommand command = new(direction, angle, jumpPower);

        jumper.Jump(command.Angle, command.Power);
    }

    /// <summary>
    /// Players are allowed to jump only if the game is still running or if 5
    /// seconds have passed since the game ended (that way players don't jump
    /// off the podiums due to a stream delay).
    /// </summary>
    private bool IsAllowedToJump()
    {
        return _timeSinceGameEnd <= 0 || (DateTime.Now.Ticks - _timeSinceGameEnd) / TimeSpan.TicksPerMillisecond > 5000;
    }

    private void SetBackground()
    {
        Sprite2D background = GetNode<Sprite2D>("Background");
        string[] colors = new string[] { "Blue", "Brown", "Gray", "Green", "Pink", "Purple", "Yellow" };
        string color = colors[_rng.RandiRange(0, colors.Length - 1)];

        background.Texture = ResourceLoader.Load<Texture2D>($"res://assets/sprites/backgrounds/{color}.png");
    }

    private FlowContainer GetEndScreenOverlay()
    {
        return GetNode<CanvasLayer>(CanvasLayerNodeName).GetNode<FlowContainer>(EndScreenOverlayNodeName);
    }

    private GameOverlay GetGameOverlay()
    {
        return GetNode<CanvasLayer>(CanvasLayerNodeName).GetNode<GameOverlay>(GameOverlayNodeName);
    }

    private LobbyOverlay GetLobbyOverlay()
    {
        return GetNode<CanvasLayer>(CanvasLayerNodeName).GetNode<LobbyOverlay>(LobbyOverlayNodeName);
    }

    private void LoadPlayerData()
    {
        string filesystemLocation = ProjectSettings.GlobalizePath(SaveLocation);

        if (!File.Exists(filesystemLocation))
        {
            return;
        }

        string jsonString = File.ReadAllText(filesystemLocation);

        try
        {
            AllPlayerData? jsonResult = JsonSerializer.Deserialize<AllPlayerData>(jsonString);

            // This can only happen if the JSON input was literally `null`.
            Ensure.IsNotNull(jsonResult);

            if (jsonResult.Players.Count == 0)
            {
                GD.PushError(
                    $"No records returned from the json string (length: {jsonString.Length}). Make sure the JSON string appears to be valid and contains data."
                );
            }

            _allPlayerData = jsonResult;
        }
        catch (JsonException e)
        {
            GD.PushError(e.Message);
        }
    }

    private void SaveAllPlayers()
    {
        string filesystemLocation = ProjectSettings.GlobalizePath(SaveLocation);
        string jsonString = JsonSerializer.Serialize(_allPlayerData);

        File.WriteAllText(filesystemLocation, jsonString);
    }

    private void GenerateLobby()
    {
        Ensure.IsNotNull(_tileSetToUse);

        Rect2 viewport = GetViewportRect();

        _heightInTiles = (int)(viewport.Size.Y / _tileSetToUse.TileSize.Y);
        _widthInTiles = (int)(viewport.Size.X / _tileSetToUse.TileSize.X);

        int floorY = _heightInTiles - 3;

        // Draw grass along the bottom to form the floor
        _lobbyTilemap.SetCell(0, new Vector2I(0, floorY), 0, new Vector2I(6, 0));
        _lobbyTilemap.SetCell(0, new Vector2I(0, floorY + 1), 0, new Vector2I(6, 1));
        _lobbyTilemap.SetCell(0, new Vector2I(0, floorY + 2), 0, new Vector2I(6, 2));

        _lobbyTilemap.SetCell(0, new Vector2I(_widthInTiles - 1, floorY), 0, new Vector2I(8, 0));
        _lobbyTilemap.SetCell(0, new Vector2I(_widthInTiles - 1, floorY + 1), 0, new Vector2I(8, 1));
        _lobbyTilemap.SetCell(0, new Vector2I(_widthInTiles - 1, floorY + 2), 0, new Vector2I(8, 2));

        for (int x = 1; x < _widthInTiles - 1; x++)
        {
            _lobbyTilemap.SetCell(0, new Vector2I(x, floorY), 0, new Vector2I(7, 0));
            _lobbyTilemap.SetCell(0, new Vector2I(x, floorY + 1), 0, new Vector2I(7, 1));
            _lobbyTilemap.SetCell(0, new Vector2I(x, floorY + 2), 0, new Vector2I(7, 2));
        }

        int wallStartY = floorY - 1;

        _ceilingHeight = wallStartY - WallHeightInTiles - 1;

        // Draw the vertical walls
        for (int y = wallStartY; y >= _ceilingHeight; y--)
        {
            _lobbyTilemap.SetCell(0, new Vector2I(0, y), 0, new Vector2I(12, 1));
            _lobbyTilemap.SetCell(0, new Vector2I(_widthInTiles - 1, y), 0, new Vector2I(12, 1));
        }

        for (int y = wallStartY; y >= _ceilingHeight - 200; y--)
        {
            _lobbyTilemap.SetCell(0, new Vector2I(0, y), 0, new Vector2I(12, 1));
            _lobbyTilemap.SetCell(0, new Vector2I(_widthInTiles - 1, y), 0, new Vector2I(12, 1));
        }

        // Draw the ceiling
        for (int x = 1; x < _widthInTiles - 1; x++)
        {
            _lobbyTilemap.SetCell(0, new Vector2I(x, _ceilingHeight), 0, new Vector2I(12, 1));
        }

        // Generate some lobby platforms
        int platformStartY = wallStartY - 2;
        int platformEndY = _ceilingHeight + 4;

        for (int y = platformStartY; y >= platformEndY; y--)
        {
            int width = _rng.RandiRange(3, 15);
            int startX = _rng.RandiRange(2, _widthInTiles - width - 2);

            AddPlatform(startX, y, width);
        }

        _generatedMaxHeight = _ceilingHeight;

        AddChild(_lobbyTilemap);
    }

    private void GenerateProceduralPlatforms()
    {
        Ensure.IsNotNull(_tileSetToUse);

        Camera2D camera = GetNode<Camera2D>(CameraNodeName);
        Rect2 viewport = GetViewportRect();

        // NOTE(Hop): GetScreenCenterPosition was the only way to get accurate viewport position
        //            without ignoring Position Smoothing
        float cameraPos = camera.GetScreenCenterPosition().Y - (viewport.Size.Y / 2);
        int cameraPosInTiles = (int)(cameraPos / _tileSetToUse.TileSize.Y);

        if (cameraPosInTiles >= _generatedMaxHeight)
        {
            return;
        }

        // Add platforms higher up;
        for (int y = _generatedMaxHeight - 1; y >= cameraPosInTiles; y--)
        {
            // This goes from 0 to 1 linearly as Y decreases
            float difficultyFactor = (float)Math.Min(0, y) / -ArenaHeightInTiles;

            // Rarely, make a solid block to add some variety
            int r = _rng.RandiRange(0, 100);

            if (r < 6 + difficultyFactor * 40)
            {
                // TODO: DrawRectangleOfTiles draws blocks _downward_, this means that part of them
                //       will suddenly appear on screen
                int blockWidth = 2 + (int)(difficultyFactor * 24);
                int blockX = _rng.RandiRange(2, _widthInTiles - 1 - blockWidth);

                DrawRectangleOfTiles(blockX, y + 1, blockWidth, blockWidth, new Vector2I(12, 1));
            }

            r = _rng.RandiRange(0, 100);

            if (r > (70 - difficultyFactor * 60))
            {
                continue;
            }

            int width = _rng.RandiRange(3, 15 - (int)Math.Round(6 * difficultyFactor));
            int startX = _rng.RandiRange(2, _widthInTiles - width - 2);

            AddPlatform(startX, y, width);
        }

        _generatedMaxHeight = cameraPosInTiles;
    }

    private void AddPlatform(int x, int y, int width)
    {
        int endX = x + width - 1;

        // Draw left side
        _lobbyTilemap.SetCell(0, new Vector2I(x, y), 0, new Vector2I(17, 1));

        // Draw middle
        if (width > 2)
        {
            for (int i = x + 1; i < endX; i++)
            {
                _lobbyTilemap.SetCell(0, new Vector2I(i, y), 0, new Vector2I(18, 1));
            }
        }

        // Draw right side
        _lobbyTilemap.SetCell(0, new Vector2I(endX, y), 0, new Vector2I(19, 1));
    }

    private void OnGameTimerDone()
    {
        GetGameOverlay().Visible = false;
        _hasGameEnded = true;
        _timeSinceGameEnd = DateTime.Now.Ticks;

        string[] winners = ComputeStats();

        SaveAllPlayers();
        ShowEndScreen(winners);
        CreateEndArena(winners);
    }

    private void CreateEndArena(string[] winners)
    {
        Ensure.IsNotNull(_tileSetToUse);

        // Draw border around the whole arena and clear any platforms
        for (int x = 0; x < _widthInTiles; x++)
        {
            for (int y = _heightInTiles; y > -_heightInTiles; y--)
            {
                if (x == 0 || x == _widthInTiles - 1 || y == _heightInTiles - 1)
                {
                    _lobbyTilemap.SetCell(0, new Vector2I(x, y), 0, new Vector2I(12, 1));
                }
                else
                {
                    _lobbyTilemap.SetCell(0, new Vector2I(x, y), -1);
                }
            }
        }

        Rect2 viewport = GetViewportRect();
        int xPadding = 100;

        // Put all players back in the arena
        for (int i = 0; i < _jumpers.Count; i++)
        {
            Jumper jumper = _jumpers.ElementAt(i).Value;

            jumper.Position = new Vector2(
                _rng.RandiRange(xPadding, (int)viewport.Size.X - xPadding),
                _rng.RandiRange((int)(viewport.Size.Y / 2), (int)viewport.Size.Y - 100)
            );
            jumper.Scale = new Vector2(1, 1);
            jumper.Velocity = new Vector2(0, 0);
        }

        // Reset the camera position
        Camera2D camera = GetNode<Camera2D>(CameraNodeName);

        camera.PositionSmoothingEnabled = false;
        camera.Position = new Vector2(0, 0);

        // Draw podiums
        int numPodiums = 3;
        int podiumWidth = 6;
        int podiumHeight = 6; // has to be divisible by numPodiums
        int podiumHeightDifference = podiumHeight / numPodiums;
        int podiumX = _widthInTiles / 2;
        int podiumY = 13 + podiumHeight;

        DrawRectangleOfTiles(podiumX, podiumY, podiumWidth, podiumHeight, new Vector2I(12, 1));
        DrawRectangleOfTiles(
            podiumX - podiumWidth,
            podiumY,
            podiumWidth,
            podiumHeight - podiumHeightDifference,
            new Vector2I(12, 1)
        );
        DrawRectangleOfTiles(
            podiumX + podiumWidth,
            podiumY,
            podiumWidth,
            podiumHeight - podiumHeightDifference * 2,
            new Vector2I(12, 1)
        );

        // Place winners on podiums
        for (int i = 0; i < winners.Length; i++)
        {
            string userId = winners[i];
            Jumper jumper = _jumpers[userId];
            int tileX = podiumX + (podiumWidth / 2);

            // This warning is only disabled due to a bug about false positives: https://github.com/SonarSource/sonar-dotnet/issues/8028
#pragma warning disable S2583 // Conditionally executed code should be reachable
            if (i == 1)
            {
                tileX -= podiumWidth;
            }
            else if (i == 2)
            {
                tileX += podiumWidth;
            }
#pragma warning restore S2583 // Conditionally executed code should be reachable

            int scale = winners.Length + 1 - i;

            jumper.Position = new Vector2(tileX * _tileSetToUse.TileSize.X, 50);
            jumper.Scale = new Vector2(scale, scale);

            jumper.SetCrazyParticles();
        }

        // Add some platforms so that there are "fun" jumps to make
        for (int y = podiumY + podiumHeight + 15; y < _heightInTiles; y += 10)
        {
            int width = _widthInTiles / 3;
            int startX = _widthInTiles / 3;

            AddPlatform(startX, y, width);
        }
    }

    private void DrawRectangleOfTiles(int leftX, int bottomY, int width, int height, Vector2I tileIndex)
    {
        for (int x = leftX; x < leftX + width; x++)
        {
            for (int y = bottomY; y > bottomY - height; y--)
            {
                _lobbyTilemap.SetCell(0, new Vector2I(x, y), 0, tileIndex);
            }
        }
    }

    private void ShowEndScreen(string[] winners)
    {
        GetEndScreenOverlay().Visible = true;

        FlowContainer endScreen = GetEndScreenOverlay();
        StringBuilder text = new("Winners:");

        text.AppendLine();

        for (int i = 0; i < winners.Length; i++)
        {
            string userId = winners[i];
            PlayerData playerData = _allPlayerData.Players[userId];
            Jumper jumper = _jumpers[userId];
            int height = GetHeightFromYPosition(jumper.Position.Y);
            string totalHeight = Formatter.FormatBigNumber(playerData.TotalHeightAchieved);

            text.Append($"\t{i + 1}: {playerData.Name}. Height reached: {height}. ");
            text.Append($"Games played: {playerData.NumPlays}. ");
            text.Append(
                $"Wins: {playerData.Num1stPlaceWins}/{playerData.Num2ndPlaceWins}/{playerData.Num3rdPlaceWins}. "
            );
            text.Append($"Lifetime height: {totalHeight}");
            text.AppendLine();
        }

        text.AppendLine();
        text.Append($"Number of players this game: {_jumpers.Count}");
        text.AppendLine().AppendLine();
        text.Append("YOU CAN NOW JUMP FREELY (until Adam gets back)!");

        endScreen.GetNode<Label>("Output").Text = text.ToString();
    }

    private List<Tuple<string, int>> GetPlayersByHeight()
    {
        List<Tuple<string, int>> playersByHeight = _jumpers
            .OrderByDescending(o => GetHeightFromYPosition(o.Value.Position.Y))
            .Select(o => new Tuple<string, int>(o.Key, GetHeightFromYPosition((int)o.Value.Position.Y)))
            .ToList();

        return playersByHeight;
    }

    private string[] ComputeStats()
    {
        List<Tuple<string, int>> playersByHeight = GetPlayersByHeight();
        string[] winners = playersByHeight.Take(3).Select(p => p.Item1).ToArray();

        for (int i = 0; i < _jumpers.Count; i++)
        {
            Jumper jumper = _jumpers.ElementAt(i).Value;
            PlayerData playerData = jumper.PlayerData;
            bool showName = false;

            playerData.NumPlays++;
            playerData.TotalHeightAchieved += GetHeightFromYPosition(jumper.Position.Y);

            if (winners.Length > 0 && winners[0] == playerData.UserId)
            {
                playerData.Num1stPlaceWins++;
                showName = true;
            }
            else if (winners.Length > 1 && winners[1] == playerData.UserId)
            {
                playerData.Num2ndPlaceWins++;
                showName = true;
            }
            else if (winners.Length > 2 && winners[2] == playerData.UserId)
            {
                playerData.Num3rdPlaceWins++;
                showName = true;
            }

            if (showName)
            {
                jumper.PlayerWon();
            }
        }

        return winners;
    }

    private void OnLobbyTimerDone()
    {
        for (int x = 1; x < _widthInTiles - 1; x++)
        {
            _lobbyTilemap.SetCell(0, new Vector2I(x, _ceilingHeight), -1);
        }

        GetLobbyOverlay().Visible = false;

        GameOverlay gameOverlay = GetGameOverlay();

        gameOverlay.Visible = true;

        gameOverlay.Init();
    }

    private void OnRedemption(object sender, OnRewardRedeemedArgs e)
    {
        if (!e.RewardId.Equals(Guid.Parse("f04bb300-d135-4670-a7ba-1d6761590042")))
        {
            return;
        }

        GD.Print($"{e.DisplayName} is redeeming a revive!");

        CallDeferred(nameof(RedeemRevive), e.DisplayName);
    }

    private void RedeemRevive(string displayName)
    {
        foreach (KeyValuePair<string, Jumper> jumpersEntry in _jumpers)
        {
            Jumper jumper = jumpersEntry.Value;

            if (jumper.PlayerData.Name.ToLower() != displayName.ToLower())
            {
                continue;
            }

            List<Tuple<string, int>> playersByHeight = GetPlayersByHeight();

            if (playersByHeight.Count <= 2)
            {
                break;
            }

            string thirdHighestPlayerId = playersByHeight[2].Item1;
            Jumper thirdHighestJumper = _jumpers[thirdHighestPlayerId];

            GD.Print("Reviving " + displayName);
            GD.Print("Snapping to " + thirdHighestJumper.PlayerData.Name);

            jumper.Position = thirdHighestJumper.Position;
            jumper.Velocity = thirdHighestJumper.Velocity;

            break;
        }
    }

    private void ModifyPlayerScales()
    {
        if (_hasGameEnded)
        {
            return;
        }

        for (int i = 0; i < _jumpers.Count; i++)
        {
            Jumper jumper = _jumpers.ElementAt(i).Value;
            int height = GetHeightFromYPosition(jumper.Position.Y);

            if (height > 0)
            {
                float scale = height / 5000f + 1;

                jumper.Scale = new Vector2(scale, scale);
            }
        }
    }

    private void MoveCamera()
    {
        Ensure.IsNotNull(_tileSetToUse);

        // Iterate over jumpers and check for the highest player
        if (_jumpers.Count == 0 || _hasGameEnded)
        {
            return;
        }

        int lowestYValue = 999999;
        string playerName = string.Empty;

        for (int i = 0; i < _jumpers.Count; i++)
        {
            Jumper jumper = _jumpers.ElementAt(i).Value;

            if (jumper.Position.Y < lowestYValue)
            {
                lowestYValue = (int)jumper.Position.Y;
                playerName = jumper.Name;
            }
        }

        int maxHeight = GetHeightFromYPosition(lowestYValue);

        // Make sure the camera doesn't go higher than 0
        int tileHeight = _tileSetToUse.TileSize.Y;

        lowestYValue = Math.Min(lowestYValue - tileHeight * 16, 0);

        Camera2D camera = GetNode<Camera2D>(CameraNodeName);

        camera.Position = new Vector2(0, lowestYValue);

        EmitSignal(SignalName.MaxHeightChanged, playerName, maxHeight);
    }

    // Y decreases as you go up, so this converts it to a "height" property that
    // increases as you go up.
    //
    // Note that ideally, the height should return 0 when you're on the lowest
    // floor, but that's probably not the case at the time of writing.
    private int GetHeightFromYPosition(float y)
    {
        return (int)(-1 * y + GetViewportRect().Size.Y);
    }
}