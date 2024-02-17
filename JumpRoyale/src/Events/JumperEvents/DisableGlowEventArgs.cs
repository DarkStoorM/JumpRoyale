using System;

namespace JumpRoyale.Events;

public class DisableGlowEventArgs(bool? dummy = null) : EventArgs
{
    public bool? Dummy { get; } = dummy;
}
