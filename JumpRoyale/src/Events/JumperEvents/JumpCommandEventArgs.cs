using System;

namespace JumpRoyale.Events;

public class JumpCommandEventArgs(int? jumpAngle, int? jumpPower) : EventArgs
{
    public int? JumpAngle { get; } = jumpAngle;

    public int? JumpPower { get; } = jumpPower;
}
