using System;

namespace JumpRoyale.Events;

public class GameTimerEventArgs(int checkpointsCount) : EventArgs
{
    public int CheckpointsCount { get; set; } = checkpointsCount;
}
