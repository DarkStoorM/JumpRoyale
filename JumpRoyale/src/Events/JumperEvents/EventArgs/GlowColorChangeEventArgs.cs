using System;

namespace JumpRoyale.Events;

public class SetGlowColorEventArgs(string? userColorChoice) : EventArgs
{
    public string? UserColorChoice { get; } = userColorChoice;
}
