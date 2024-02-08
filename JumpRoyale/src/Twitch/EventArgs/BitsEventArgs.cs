using System;
using TwitchLib.PubSub.Events;

public class BitsEventArgs(OnBitsReceivedArgs eventArgs) : EventArgs
{
    public int BitsAmount { get; init; } = eventArgs.BitsUsed;

    public string UserId { get; init; } = eventArgs.UserId;
}
