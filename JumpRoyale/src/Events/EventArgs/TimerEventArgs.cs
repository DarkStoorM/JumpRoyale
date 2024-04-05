using System;

namespace JumpRoyale.Events;

public class EventTimerEventArgs(int checkpointsCount) : EventArgs
{
    public int CheckpointsCount { get; set; } = checkpointsCount;
}
