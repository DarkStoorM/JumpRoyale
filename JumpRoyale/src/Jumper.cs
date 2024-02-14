using System;

namespace JumpRoyale;

public class Jumper(PlayerData playerData)
{
    public PlayerData PlayerData { get; } = playerData;

    /// <summary>
    /// Currently evaluated jump angle from the JumpCommand.
    /// </summary>
    public int JumpAngle { get; }

    /// <summary>
    /// Currently evaluated jump power from the JumpCommand.
    /// </summary>
    public int JumpPower { get; }

    /// <summary>
    /// Executes the Jump on player's Jumper.
    /// </summary>
    /// <param name="direction">Direction alias.</param>
    /// <param name="jumpAngle">Jump angle specified by the user (-90° - 90°) in his chat message.</param>
    /// <param name="jumpPower">Jump power specified by the user in his chat message.</param>
    public void ExecuteJump(string direction, int? jumpAngle, int? jumpPower)
    {
        Console.WriteLine($"{direction} {jumpAngle} {jumpPower}");
    }

    /// <summary>
    /// Disables the particles on this player.
    /// </summary>
    public void DisableGlow()
    {
        Console.WriteLine("Disabling glow");
    }

    /// <summary>
    /// Changes the player's appearance by selecting a new character sprite.
    /// </summary>
    /// <param name="characterChoice">Numeric choice specified in the chat message.</param>
    public void SetCharacter(int? characterChoice)
    {
        Console.WriteLine($"{characterChoice}");
    }

    /// <summary>
    /// Enables the particles for the player.
    /// </summary>
    /// <param name="userColorChoice">Glow color specified by the user.</param>
    /// <param name="fallbackColor">Default glow color to fallback to, which is Twitch Chat color.</param>
    public void SetGlowColor(string? userColorChoice, string fallbackColor)
    {
        Console.WriteLine($"{userColorChoice} {fallbackColor}");
    }

    /// <summary>
    /// Modifies the color of player's Nameplate in-game.
    /// </summary>
    /// <param name="userColorChoice">Name color selected by the user.</param>
    /// <param name="fallbackColor">Default color to fallback to, which is the Twitch Chat Color.</param>
    public void SetNameColor(string? userColorChoice, string fallbackColor)
    {
        Console.WriteLine($"{userColorChoice} {fallbackColor}");
    }
}
