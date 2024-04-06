using System;

namespace JumpRoyale.Events;

public class EventTimerEventArgs(int eventsRaisedCount) : EventArgs
{
    public int EventsRaisedCount { get; set; } = eventsRaisedCount;
}
