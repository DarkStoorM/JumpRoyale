using System;
using JumpRoyale.Utils;

namespace JumpRoyale.Commands;

public class ChatCommandHandler(string message, string userId, string displayName, string colorHex, bool isPrivileged)
{
    public string Message { get; } = message.ToLower();

    public string DisplayName { get; } = displayName;

    public string UserId { get; } = userId;

    public string ColorHex { get; } = colorHex;

    public bool IsPrivileged { get; } = isPrivileged;

    public GenericActionHandler<Jumper>? CallableCommand { get; }

    public ChatCommandParser ExecutedCommand { get; private set; } = null!;

    public void ProcessMessage()
    {
        GenericActionHandler<Jumper>? callableCommand = TryGetCommandFromChatMessage();

        if (callableCommand is null)
        {
            return;
        }

        // Small workaround to call Join before other commands, because we have to let it populate the Jumpers
        // dictionary with players
        if (ExecutedCommand.Name.Equals("join"))
        {
            // In the join command, we don't care if the jumper exists or not, we don't need him here, hence the `null!`
            callableCommand(null!);
            return;
        }

        // Retrieve the Jumper instance associated with the chatter and execute the command if he has joined the game
        Jumper? jumper = PlayerStats.Instance.GetJumperByUserId(UserId);

        if (jumper is null)
        {
            return;
        }

        callableCommand(jumper);
    }

    /// <summary>
    /// Returns a command delegate if the chat message contained a valid command name.
    /// </summary>
    public GenericActionHandler<Jumper>? TryGetCommandFromChatMessage()
    {
        ExecutedCommand = new(Message);

        string?[] stringArguments = ExecutedCommand.ArgumentsAsStrings();
        int?[] numericArguments = ExecutedCommand.ArgumentsAsNumbers();

        // Join is the only command that can be executed by everyone, whether joined or not.
        // All the remaining commands are only available to those who joined the game
        if (ChatCommandMatcher.MatchesJoin(ExecutedCommand.Name))
        {
            return (jumper) => HandleJoin(UserId, DisplayName, ColorHex, IsPrivileged);
        }

        // Important: when working with aliases that collide with each other, remember to use the
        // proper order. E.g. Jump has `u` alias and if it was first on the list, it would
        // execute if `unglow` was sent in the chat, because we don't use exact matching
        return ExecutedCommand.Name switch
        {
            // -- Commands for all Chatters (active)
            string when ChatCommandMatcher.MatchesUnglow(ExecutedCommand.Name) => (jumper) => jumper.DisableGlow(),
            string when ChatCommandMatcher.MatchesJump(ExecutedCommand.Name)
                => (jumper) => jumper.ExecuteJump(ExecutedCommand.Name, numericArguments[0], numericArguments[1]),
            string when ChatCommandMatcher.MatchesCharacterChange(ExecutedCommand.Name)
                => (jumper) => jumper.SetCharacter(numericArguments[0]),

            // -- Commands for Mods, VIPs, Subs
            string when ChatCommandMatcher.MatchesGlow(ExecutedCommand.Name, IsPrivileged)
                => (jumper) => jumper.SetGlowColor(stringArguments[0], ColorHex),
            string when ChatCommandMatcher.MatchesNamecolor(ExecutedCommand.Name, IsPrivileged)
                => (jumper) => jumper.SetNameColor(stringArguments[0], ColorHex),
            _ => null,
        };
    }

    /// <summary>
    /// Creates a new Jumper object for the player.
    /// </summary>
    /// <param name="userId">Twitch User Id.</param>
    /// <param name="displayName">Twitch Display Name.</param>
    /// <param name="colorHex">Twitch Chat color.</param>
    /// <param name="isPrivileged">If this player privileged for extra features, enables them on join.</param>
    private void HandleJoin(string userId, string displayName, string colorHex, bool isPrivileged)
    {
        PlayerData? playerData = PlayerStats.Instance.GetPlayerById(userId);
        Jumper? jumper = PlayerStats.Instance.GetJumperByUserId(userId);

        // If this player has already joined the game, don't do anything. Create a new Jumper if he's actually joining
        if (jumper is not null)
        {
            return;
        }

        // If this player has not joined the game yet, create a data object for this player and store privileges
        playerData ??= new(colorHex, Rng.IntRange(1, 18), colorHex, userId);

        // Update the PlayerData with new data, which could potentially change between the game sessions, e.g. player's
        // username, his sub status
        playerData.Initialize(displayName, isPrivileged, colorHex);

        jumper = new(playerData);
        jumper.Initialize();

        // Update both objects. Probably should make some relation to do both in one call...
        PlayerStats.Instance.UpdatePlayer(playerData);
        PlayerStats.Instance.UpdateJumper(jumper);
        PlayerStats.Instance.EmitPlayerJoinEvent(jumper);

        // To Be Added:
        // 1) place Jumper on the scene within arena bounds (requires new PR with Arena Generation)
    }
}
