using System;
using JumpRoyale.Utils;
using TwitchLib.PubSub.Events;

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
