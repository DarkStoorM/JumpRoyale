using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using JumpRoyale.Utils.Exceptions;

namespace JumpRoyale;

/// <summary>
/// Class responsible for loading Player Data from specified Json file (serialization and deserialization).
/// </summary>
public class PlayerStats
{
    private static readonly object _lock = new();
    private static PlayerStats? _instance;

    /// <summary>
    /// Gets currently deserialized Json data of all players.
    /// </summary>
    private readonly AllPlayerData _allPlayerData = new();

    private PlayerStats(string pathToStatsFile)
    {
        StatsFilePath = pathToStatsFile;
    }

    public static PlayerStats Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance is null)
                {
                    throw new InvalidOperationException("Call Initialize() before using PlayerStats.");
                }

                return _instance;
            }
        }
    }

    /// <summary>
    /// Defines where the path for player stats is located.
    /// </summary>
    public string StatsFilePath { get; set; }

    /// <summary>
    /// Initializes the PlayerStats with provided stats file path. This path is required to point into a valid location,
    /// where the file will be stored/read from.
    /// </summary>
    /// <param name="pathToStatsFile">Path to the "save" file with serialized player statistics.</param>
    public static void Initialize(string pathToStatsFile)
    {
        if (pathToStatsFile is null || pathToStatsFile.Length == 0)
        {
            throw new MissingStatsFilePathException();
        }

        lock (_lock)
        {
            _instance ??= new PlayerStats(pathToStatsFile);
        }
    }

    /// <summary>
    /// Destroys the currently existing singleton for refreshing purposes, e.g. Initializing with different file paths
    /// in separate scenes or during testing.
    /// </summary>
    public static void Destroy()
    {
        _instance = null;
    }

    /// <summary>
    /// Clears the Players dictionary.
    /// </summary>
    public void ClearPlayers()
    {
        _allPlayerData.Players.Clear();
    }

    /// <summary>
    /// Returns true if the player with specified Twitch id exists in the collection.
    /// </summary>
    /// <param name="userId">User Twitch id.</param>
    public bool Exists(string userId)
    {
        return _allPlayerData.Players.ContainsKey(userId);
    }

    /// <summary>
    /// Returns PlayerData indexed by specified player id, if he exists in the dictionary loaded from Json file.
    /// </summary>
    /// <param name="playerId">Twitch User id.</param>
    public PlayerData? GetPlayerById(string playerId)
    {
        _allPlayerData.Players.TryGetValue(playerId, out PlayerData? playerData);

        return playerData;
    }

    /// <summary>
    /// Attempts to read all the player data from Json file and stores it internally in <see cref="AllPlayerData"/>. If
    /// there was no data inside (new file or empty object), returns early with state. It will only throw an exception
    /// if the json was malformed or the structure didn't match the type.
    /// </summary>
    public bool LoadPlayerData()
    {
        if (!File.Exists(StatsFilePath))
        {
            return false;
        }

        string jsonString = File.ReadAllText(StatsFilePath);

        AllPlayerData? jsonResult = JsonSerializer.Deserialize<AllPlayerData>(jsonString);

        if (jsonResult is null)
        {
            return false;
        }

        // If there was data in the json file, but we didn't get anything out of it, it probably means that main
        // "players" property was changed, so we didn't match the required structure. Not sure what's the optimal length
        // to test, but probably longer than: "{}". Maybe it doesn't matter here at all.
        if (jsonResult.Players.Count == 0 && jsonString.Length > 4)
        {
            throw new InvalidJsonDataException();
        }

        foreach (KeyValuePair<string, PlayerData> player in jsonResult.Players)
        {
            _allPlayerData.Players.Add(player.Key, player.Value);
        }

        return true;
    }

    /// <summary>
    /// Serializes all players and stores them in the Json file.
    /// </summary>
    public void SaveAllPlayers()
    {
        string jsonString = JsonSerializer.Serialize(_allPlayerData);

        File.WriteAllText(StatsFilePath, jsonString);
    }

    /// <summary>
    /// Updates (or stores) the indexed player with new player data. Automatically keyed by the <c>userId</c> from
    /// provided <c>playerData</c>.
    /// <para>
    /// Note: This was merged with Store, which called <c>dictionary.Add()</c>, but that requires exception handling,
    /// but we can just directly access the index, which automatically overwrites the existing record. <c>UserId</b> is
    /// unique anyway, so there is no way to get a duplicate unless we spawn bots with hardcoded ids.
    /// </para>
    /// </summary>
    /// <remarks>
    /// This only updates the dictionary entry, not the record in Json file, this is handled later during serialization.
    /// </remarks>
    /// <param name="playerData">New player data.</param>
    public void UpdatePlayer(PlayerData playerData)
    {
        if (playerData is null)
        {
            throw new NullPlayerDataException();
        }

        _allPlayerData.Players[playerData.UserId] = playerData;
    }
}
