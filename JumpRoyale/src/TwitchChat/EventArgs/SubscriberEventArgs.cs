using System;
using JumpRoyale.Utils;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;

namespace TwitchChat;

/// <summary>
/// Selected properties of Subscription events. This combines the following: <c>OnNewSubscriberArgs</c>,
/// <c>OnReSubscriberArgs</c>, <c>OnPrimePaidSubscriberArgs</c>.
/// </summary>
public class SubscriberEventArgs : EventArgs
{
    public SubscriberEventArgs(OnNewSubscriberArgs eventArgs)
    {
        NullGuard.ThrowIfNull(eventArgs);

        ColorHex = eventArgs.Subscriber.ColorHex;
        DisplayName = eventArgs.Subscriber.DisplayName;
        SubscriptionPlan = eventArgs.Subscriber.SubscriptionPlan;
        UserId = eventArgs.Subscriber.UserId;
    }

    public SubscriberEventArgs(OnReSubscriberArgs eventArgs)
    {
        NullGuard.ThrowIfNull(eventArgs);

        ColorHex = eventArgs.ReSubscriber.ColorHex;
        DisplayName = eventArgs.ReSubscriber.DisplayName;
        SubscriptionPlan = eventArgs.ReSubscriber.SubscriptionPlan;
        UserId = eventArgs.ReSubscriber.UserId;
    }

    public SubscriberEventArgs(OnPrimePaidSubscriberArgs eventArgs)
    {
        NullGuard.ThrowIfNull(eventArgs);

        ColorHex = eventArgs.PrimePaidSubscriber.ColorHex;
        DisplayName = eventArgs.PrimePaidSubscriber.DisplayName;
        SubscriptionPlan = eventArgs.PrimePaidSubscriber.SubscriptionPlan;
        UserId = eventArgs.PrimePaidSubscriber.UserId;
    }

    public string DisplayName { get; init; }

    public string UserId { get; init; }

    public string ColorHex { get; init; }

    public SubscriptionPlan SubscriptionPlan { get; init; }
}
