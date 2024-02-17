using System;
using JumpRoyale.Commands;

namespace JumpRoyale.Events;

public class JumpCommandEventArgs(JumpCommand jumpCommand) : EventArgs
{
    public int JumpAngle { get; } = jumpCommand.Angle;

    public int JumpPower { get; } = jumpCommand.Power;
}
