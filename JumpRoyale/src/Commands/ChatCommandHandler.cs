using Godot;
using Utils;

namespace JumpRoyale;

public class CommandHandler(string message, string userId, string displayName, string colorHex, bool isPrivileged)
{
    public string Message { get; } = message.ToLower();

    public string DisplayName { get; } = userId;

    public string UserId { get; } = displayName;

    public string ColorHex { get; } = colorHex;

    public bool IsPrivileged { get; } = isPrivileged;

    public GenericActionHandler<object>? CallableCommand { get; }

    public ChatCommandParser ExecutedCommand { get; private set; } = null!;

    public void ProcessMessage()
    {
        GenericActionHandler<object>? callableCommand = TryGetCommandFromChatMessage();

        if (callableCommand is null)
        {
            return;
        }

        // Small workaround to call Join before other commands, because we have to let it populate the Jumpers
        // dictionary with players
        if (ExecutedCommand.Name.Equals("join"))
        {
            // Automatically dispose unused object, this is only here because we need to match the delegate, even though
            // the jumper is not required in the Join command
            callableCommand(new());
        }

        // Retrieve the Jumper instance and execute the command
        // Jumper jumper = ActiveJumpers.Instance.GetById(_senderId);
        // command(jumper);
    }

    /// <summary>
    /// Returns a command delegate if the chat message contained a valid command name.
    /// </summary>
    public GenericActionHandler<object>? TryGetCommandFromChatMessage()
    {
        ExecutedCommand = new(Message);

        string?[] stringArguments = ExecutedCommand.ArgumentsAsStrings();
        int?[] numericArguments = ExecutedCommand.ArgumentsAsNumbers();

        // Join is the only command that can be executed by everyone, whether joined or not.
        // All the remaining commands are only available to those who joined the game
        if (CommandMatcher.MatchesJoin(ExecutedCommand.Name))
        {
            return (jumper) => HandleJoin(UserId, DisplayName, ColorHex, IsPrivileged);
        }

        // Important: when working with aliases that collide with each other, remember to use the
        // proper order. E.g. Jump has `u` alias and if it was first on the list, it would
        // execute if `unglow` was sent in the chat, because we don't use exact matching
        return ExecutedCommand.Name switch
        {
            // -- Commands for all Chatters (active)
            string when CommandMatcher.MatchesUnglow(ExecutedCommand.Name) => (jumper) => HandleUnglow(jumper),
            string when CommandMatcher.MatchesJump(ExecutedCommand.Name)
                => (jumper) => HandleJump(jumper, ExecutedCommand.Name, numericArguments[0], numericArguments[1]),
            string when CommandMatcher.MatchesCharacterChange(ExecutedCommand.Name)
                => (jumper) => HandleCharacterChange(jumper, numericArguments[0]),

            // -- Commands for Mods, VIPs, Subs
            string when CommandMatcher.MatchesGlow(ExecutedCommand.Name, IsPrivileged)
                => (jumper) => HandleGlow(jumper, stringArguments[0], ColorHex),
            string when CommandMatcher.MatchesNamecolor(ExecutedCommand.Name, IsPrivileged)
                => (jumper) => HandleNamecolor(jumper, stringArguments[0], ColorHex),
            _ => null,
        };
    }

    /// <summary>
    /// Changes the player's appearance by selecting a new character sprite.
    /// </summary>
    /// <param name="jumper">Player's Jumper object.</param>
    /// <param name="characterChoice">Numeric choice specified in the chat message.</param>
    private void HandleCharacterChange(object jumper, int? characterChoice)
    {
        GD.Print("HandleCharacterChange called");
    }

    /// <summary>
    /// Enables the particles for the player.
    /// </summary>
    /// <param name="jumper">Player's Jumper object.</param>
    /// <param name="userGlowColor">Glow color specified by the user.</param>
    /// <param name="defaultGlowColor">Default glow color to fallback to, which is Twitch Chat color.</param>
    private void HandleGlow(object jumper, string? userGlowColor, string defaultGlowColor)
    {
        GD.Print("HandleGlow called");
    }

    /// <summary>
    /// Creates a new Jumper object for the player.
    /// </summary>
    /// <param name="userID">Twitch User Id.</param>
    /// <param name="displayName">Twitch Display Name.</param>
    /// <param name="colorHex">Twitch Chat color.</param>
    /// <param name="isPrivileged">If this player privileged for extra features, enables them on join.</param>
    private void HandleJoin(string userID, string displayName, string colorHex, bool isPrivileged)
    {
        GD.Print("HandleJoin called");
    }

    /// <summary>
    /// Executes the Jump on player's Jumper.
    /// </summary>
    /// <param name="jumper">Player's Jumper object.</param>
    /// <param name="direction">Direction alias.</param>
    /// <param name="angle">Jump angle specified by the user (-90° - 90°) in his chat message.</param>
    /// <param name="power">Jump power specified by the user in his chat message.</param>
    private void HandleJump(object jumper, string direction, int? angle, int? power)
    {
        GD.Print("HandleJump called");
    }

    /// <summary>
    /// Disables the particles on this player.
    /// </summary>
    /// <param name="jumper">Player's Jumper object.</param>
    private void HandleUnglow(object jumper)
    {
        GD.Print("HandleUnglow called");
    }

    /// <summary>
    /// Modifies the color of player's Nameplate in-game.
    /// </summary>
    /// <param name="jumper">Player's Jumper object.</param>
    /// <param name="userNameColor">Name color selected by the user.</param>
    /// <param name="defaultNameColor">Default color to fallback to, which is the Twitch Chat Color.</param>
    private void HandleNamecolor(object jumper, string? userNameColor, string defaultNameColor)
    {
        GD.Print("HandleNamecolor called");
    }
}
