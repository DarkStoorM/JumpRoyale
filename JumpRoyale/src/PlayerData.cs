using System.Text.Json.Serialization;

namespace JumpRoyale;

// Note to contributors: do not rename anything in this class without updating the corresponding JSON data on disk.
public class PlayerData(string glowColor, int characterChoice, string nameColor)
{
    public int CharacterChoice { get; set; } = characterChoice;

    public string GlowColor { get; set; } = glowColor;

    /// <summary>
    /// Defines the highest amount of consecutive first place game results.
    /// </summary>
    public int HighestWinStreak { get; set; }

    /// <summary>
    /// Defines this player's current <c>isPrivileged</c> status that was set upon joining the game. This property is
    /// mostly used by Join command to instantiate the player with previously customized privileged features, but it can
    /// also be used for commands, that are partially privileged, like extra cosmetics inside a command.
    /// <para>
    /// This property will not be a part of the Json data.
    /// </para>
    /// <para>
    /// Example: <c>char</c> command allows to select a character from given range, but some selections could only be
    /// applied to privileged users.
    /// </para>
    /// </summary>
    [JsonIgnore]
    public bool IsPrivileged { get; set; } = false;

    // Note that this can change between games since you can change your name on
    // Twitch, so this is just for convenience of looking at the JSON file and
    // knowing who's who.
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Current name color selected by the player. This will be used to automatically revert to default color if this
    /// player was unprivileged or to use it again when privileged so we don't overwrite it with default color.
    /// </summary>
    /// <remarks>
    /// This property is for Serialization purposes only and should not be used. If you need to return the color
    /// selected by players only if they are privileged, use <see cref="PlayerNameColor"/>.
    /// </remarks>
    public string NameColor { get; private set; } = nameColor ?? "ffffff";

    public int Num1stPlaceWins { get; set; }

    public int Num2ndPlaceWins { get; set; }

    public int Num3rdPlaceWins { get; set; }

    public int NumJumps { get; set; }

    public int NumPlays { get; set; }

    /// <summary>
    /// Gets the current name color selected by the player, but will return default color if this player was
    /// unprivileged.
    /// </summary>
    /// <remarks>
    /// This property will not be serialized.
    /// </remarks>
    [JsonIgnore]
    public string PlayerNameColor
    {
        get => IsPrivileged ? NameColor : "ffffff";
        set { NameColor = value; }
    }

    /// <summary>
    /// Cumulative height from all game sessions.
    /// </summary>
    public int TotalHeightAchieved { get; set; }

    /// <summary>
    /// User Twitch id.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Defines the current win streak (1st place only). Increments every time the player achieves the first place in
    /// the current session, resets to 0 otherwise.
    /// </summary>
    public int WinStreak { get; set; }

    /// <summary>
    /// Initializes the dynamically imported values with the new data, which could potentially change at any point.
    /// </summary>
    /// <param name="name">Twitch display name.</param>
    /// <param name="isPrivileged">Current privilege status (subscriber, mod, etc).</param>
    /// <param name="twitchChatColor">Fallback color to set if player had no color assigned (name or glow).</param>
    public void Initialize(string name, bool isPrivileged, string twitchChatColor)
    {
        // Update the player name in case his Twitch name has changed. Also, update the remaining properties
        Name = name;
        IsPrivileged = isPrivileged;

        // Update the following only if the values didn't exist in the save file
        PlayerNameColor ??= twitchChatColor;
        GlowColor ??= twitchChatColor;
    }
}
