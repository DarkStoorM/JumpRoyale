using Godot;

namespace JumpRoyale.Utils;

/// <summary>
/// Struct of Label for UI stats purposes: player name / current Y-position.
/// </summary>
public readonly struct PodiumJumperLabels(Label playerNameLabel, Label playerHeightLabel)
{
    /// <summary>
    /// UI part of the leaders live feed where the player's name is displayed.
    /// </summary>
    public Label PlayerNameLabel { get; } = playerNameLabel;

    /// <summary>
    /// UI part of the leaders live feed where the player's current Y-position is displayed.
    /// </summary>
    public Label PlayerHeightLabel { get; } = playerHeightLabel;
}
