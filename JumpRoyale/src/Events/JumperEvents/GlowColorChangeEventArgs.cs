using System;

namespace JumpRoyale.Events;

public class GlowColorChangeEventArgs(string? userColorChoice, string? fallbackColor) : EventArgs
{
    public string? UserColorChoice { get; } = userColorChoice;

    public string? FallbackColor { get; } = fallbackColor;
}
