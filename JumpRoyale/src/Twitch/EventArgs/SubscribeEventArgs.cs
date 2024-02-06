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

        ColorHex = eventArgs.Subscriber.ColorHex;
        DisplayName = eventArgs.Subscriber.DisplayName;
        SubscriptionPlan = eventArgs.Subscriber.SubscriptionPlan;
        UserId = eventArgs.Subscriber.UserId;
    }

    public SubscribeEventArgs(OnReSubscriberArgs eventArgs)
    {
        NullGuard.ThrowIfNull(eventArgs);

        ColorHex = eventArgs.ReSubscriber.ColorHex;
        DisplayName = eventArgs.ReSubscriber.DisplayName;
        SubscriptionPlan = eventArgs.ReSubscriber.SubscriptionPlan;
        UserId = eventArgs.ReSubscriber.UserId;
    }

    public SubscribeEventArgs(OnPrimePaidSubscriberArgs eventArgs)
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
