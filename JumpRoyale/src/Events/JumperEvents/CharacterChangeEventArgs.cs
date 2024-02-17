using System;

namespace JumpRoyale.Events;

public class CharacterChangeEventArgs(int? userCharacterChoice) : EventArgs
{
    public int? UserCharacterChoice { get; } = userCharacterChoice;
}
