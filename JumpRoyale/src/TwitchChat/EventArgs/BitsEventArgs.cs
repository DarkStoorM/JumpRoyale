using System;
using TwitchLib.PubSub.Events;

namespace TwitchChat;

/// <summary>
/// Selected properties of <c>OnBitsReceivedArgs</c>.
/// </summary>
public class BitsEventArgs(OnBitsReceivedArgs eventArgs) : EventArgs
{
    public int BitsAmount { get; init; } = eventArgs.BitsUsed;

    public string UserId { get; init; } = eventArgs.UserId;
}
