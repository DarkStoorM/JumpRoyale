using System;

namespace JumpRoyale.Events;

public class TimerEventArgs(int checkpointsCount) : EventArgs
{
    public int CheckpointsCount { get; set; } = checkpointsCount;
}
