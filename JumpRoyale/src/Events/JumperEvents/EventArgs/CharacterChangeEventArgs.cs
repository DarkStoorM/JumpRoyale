using System;

namespace JumpRoyale.Events;

public class SetCharacterEventArgs(int? userCharacterChoice) : EventArgs
{
    public int? UserCharacterChoice { get; } = userCharacterChoice;
}
