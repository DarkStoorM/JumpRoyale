using System;

namespace JumpRoyale.Events;

public class SetNameColorEventArgs(string? userColorChoice) : EventArgs
{
    public string? UserColorChoice { get; } = userColorChoice;
}
