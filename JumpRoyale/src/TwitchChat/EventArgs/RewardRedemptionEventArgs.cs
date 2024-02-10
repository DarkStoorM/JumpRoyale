using System;
using TwitchLib.PubSub.Events;
using Utils;

namespace TwitchChat;

/// <summary>
/// Selected properties of <c>OnRewardRedeemedArgs</c>.
/// </summary>
public class RewardRedemptionEventArgs : EventArgs
{
    public RewardRedemptionEventArgs(OnRewardRedeemedArgs eventArgs)
    {
        NullGuard.ThrowIfNull(eventArgs);

        DisplayName = eventArgs.DisplayName;
        RedemptionId = eventArgs.RedemptionId;
    }

    public string DisplayName { get; }

    public Guid RedemptionId { get; }
}
