using System;
using JumpRoyale.Commands;
using JumpRoyale.Events;
using JumpRoyale.Utils;

namespace JumpRoyale;

/// <summary>
/// Jumper Object class responsible for raising events invoked by the Command Handler, if appropriate, matching command
/// has been detected in the user's chat message. This class exposes a special event manager that Scenes or other
/// classes can subscribe to in order to execute their own logic if this manager raises a specific event.
/// </summary>
public class Jumper(PlayerData playerData)
{
    public PlayerData PlayerData { get; } = playerData;

    public JumperEventsManager JumperEventsManager { get; } = new();

    /// <summary>
    /// Executes the Jump on player's Jumper.
    /// </summary>
    /// <param name="direction">Direction alias.</param>
    /// <param name="jumpAngle">Jump angle specified by the user (-90° - 90°) in his chat message.</param>
    /// <param name="jumpPower">Jump power specified by the user in his chat message.</param>
    public void ExecuteJump(string direction, int? jumpAngle, int? jumpPower)
    {
        JumpCommand jump = new(direction, jumpAngle, jumpPower);

        JumperEventsManager.InvokeEvent(JumperEventTypes.OnJumpCommandEvent, new JumpCommandEventArgs(jump));
    }

    /// <summary>
    /// Disables the particles on this player.
    /// </summary>
    public void DisableGlow()
    {
        JumperEventsManager.InvokeEvent(JumperEventTypes.OnDisableGlow, new DisableGlowEventArgs());
    }

    /// <summary>
    /// Changes the player's appearance by selecting a new character sprite.
    /// </summary>
    /// <param name="characterChoice">Numeric choice specified in the chat message.</param>
    public void SetCharacter(int? characterChoice)
    {
        int choice = Math.Clamp(characterChoice ?? Rng.IntRange(1, 18), 1, 18);

        PlayerData.CharacterChoice = choice;

        JumperEventsManager.InvokeEvent(JumperEventTypes.OnSetCharacter, new SetCharacterEventArgs(choice));
    }

    /// <summary>
    /// Enables the particles for the player.
    /// </summary>
    /// <remarks>
    /// This method invokes OnSetGlowColor event.
    /// </remarks>
    /// <param name="userColorChoice">Glow color specified by the user.</param>
    /// <param name="fallbackColor">Default glow color to fallback to, which is Twitch Chat color.</param>
    public void SetGlowColor(string? userColorChoice, string fallbackColor)
    {
        string glowColor = userColorChoice ?? fallbackColor;

        PlayerData.GlowColor = glowColor;

        JumperEventsManager.InvokeEvent(JumperEventTypes.OnSetGlowColor, new SetGlowColorEventArgs(glowColor));
    }

    /// <summary>
    /// Modifies the color of player's Nameplate in-game.
    /// </summary>
    /// <remarks>
    /// This method invokes OnSetNameColor event.
    /// </remarks>
    /// <param name="userColorChoice">Name color selected by the user.</param>
    /// <param name="fallbackColor">Default color to fallback to, which is the Twitch Chat Color.</param>
    public void SetNameColor(string? userColorChoice, string fallbackColor)
    {
        string nameColor = userColorChoice ?? fallbackColor;

        PlayerData.PlayerNameColor = nameColor;

        JumperEventsManager.InvokeEvent(JumperEventTypes.OnSetNameColor, new SetNameColorEventArgs(nameColor));
    }
}
