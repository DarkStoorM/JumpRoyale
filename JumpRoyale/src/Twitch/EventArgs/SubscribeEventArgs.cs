using System;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using Utils;

namespace TwitchChat;

public class SubscribeEventArgs : EventArgs
{
    public SubscribeEventArgs(OnNewSubscriberArgs eventArgs)
    {
        NullGuard.ThrowIfNull(eventArgs);

        DisplayName = eventArgs.Subscriber.DisplayName;
        UserId = eventArgs.Subscriber.UserId;
        SubscriptionPlan = eventArgs.Subscriber.SubscriptionPlan;
    }

    public SubscribeEventArgs(OnReSubscriberArgs eventArgs)
    {
        NullGuard.ThrowIfNull(eventArgs);

        DisplayName = eventArgs.ReSubscriber.DisplayName;
        UserId = eventArgs.ReSubscriber.UserId;
        SubscriptionPlan = eventArgs.ReSubscriber.SubscriptionPlan;
    }

    public SubscribeEventArgs(OnPrimePaidSubscriberArgs eventArgs)
    {
        NullGuard.ThrowIfNull(eventArgs);

        DisplayName = eventArgs.PrimePaidSubscriber.DisplayName;
        UserId = eventArgs.PrimePaidSubscriber.UserId;
        SubscriptionPlan = eventArgs.PrimePaidSubscriber.SubscriptionPlan;
    }

    public string DisplayName { get; init; }

    public string UserId { get; init; }

    public SubscriptionPlan SubscriptionPlan { get; init; }
}
