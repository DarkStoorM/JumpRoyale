using System;
using TwitchLib.PubSub.Events;
using Utils;

namespace TwitchChat;

/// <summary>
/// Selected event arguments from OnRewardRedeemedArgs.
/// </summary>
public class RewardRedemptionEventArgs : EventArgs
{
    public RewardRedemptionEventArgs(OnRewardRedeemedArgs eventArgs)
    {
        NullGuard.ThrowIfNull<ArgumentNullException>(eventArgs);

        DisplayName = eventArgs.DisplayName;
        RewardId = eventArgs.RedemptionId.ToString();
    }

    public string DisplayName { get; init; }

    public string RewardId { get; init; }
}
