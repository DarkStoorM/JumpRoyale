using System;

namespace JumpRoyale.Events;

public class PlayerJoinEventArgs(Jumper jumper) : EventArgs
{
    public Jumper Jumper { get; } = jumper;
}
