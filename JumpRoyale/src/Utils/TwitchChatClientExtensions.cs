using System;
using System.Reflection;
using TwitchChat;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Builders;
using TwitchLib.PubSub.Events;
using Utils;

public static class TwitchChatClientExtensions
{
    /// <summary>
    /// Invokes a Chat Message Event and allows replacing some components with custom data if more specific information
    /// has to be passed to the Client's event handlers.
    /// </summary>
    public static void InvokeFakeMessageEvent(
        this TwitchChatClient client,
        string? message = null,
        string? displayName = null,
        string? userId = null,
        string? colorHex = null
    )
    {
        ChatMessageBuilder messageBuilder = ChatMessageBuilder.Create();
        TwitchLibMessageBuilder libMessageBuilder = TwitchLibMessageBuilder.Create();

        messageBuilder.WithMessage(message ?? "FakeMessage");
        libMessageBuilder
            .WithDisplayName(displayName ?? "FakeName")
            .WithUserId(userId ?? "FakeId")
            .WithColorHex(colorHex ?? "FakeColor");

        ChatMessage chatMessage = messageBuilder.WithTwitchLibMessage(libMessageBuilder).Build();
        OnMessageReceivedArgs messageArgs = new() { ChatMessage = chatMessage };

        TwitchChatClient.Instance.ManuallyInvokeMessageEvent(messageArgs);
    }

    /// <summary>
    /// Invokes a New Subscriber Event and allows replacing some components with custom data if more specific
    /// information has to be passed to the Client's event handlers.
    /// </summary>
    public static void InvokeFakeNewSubscriberEvent(
        this TwitchChatClient client,
        string? displayName = null,
        string? userId = null,
        SubscriptionPlan? subscriptionPlan = null
    )
    {
        OnNewSubscriberArgs newSubArgs = CreateSubscriberArgs<Subscriber, OnNewSubscriberArgs>(
            displayName,
            userId,
            subscriptionPlan
        );

        TwitchChatClient.Instance.ManuallyInvokeNewSubscriberEvent(newSubArgs);
    }

    /// <summary>
    /// Invokes a ReSubscriber Event and allows replacing some components with custom data if more specific
    /// information has to be passed to the Client's event handlers.
    /// </summary>
    public static void InvokeFakeReSubscriberEvent(
        this TwitchChatClient client,
        string? displayName = null,
        string? userId = null,
        SubscriptionPlan? subscriptionPlan = null
    )
    {
        OnReSubscriberArgs resubArgs = CreateSubscriberArgs<ReSubscriber, OnReSubscriberArgs>(
            displayName,
            userId,
            subscriptionPlan
        );

        TwitchChatClient.Instance.ManuallyInvokeReSubscriberEvent(resubArgs);
    }

    /// <summary>
    /// Invokes a ReSubscriber Event and allows replacing some components with custom data if more specific
    /// information has to be passed to the Client's event handlers.
    /// </summary>
    public static void InvokeFakePrimeSubscriberEvent(
        this TwitchChatClient client,
        string? displayName = null,
        string? userId = null
    )
    {
        OnPrimePaidSubscriberArgs primeSubArgs = CreateSubscriberArgs<PrimePaidSubscriber, OnPrimePaidSubscriberArgs>(
            displayName,
            userId,
            SubscriptionPlan.Prime
        );

        TwitchChatClient.Instance.ManuallyInvokePrimeSubscriberEvent(primeSubArgs);
    }

    /// <summary>
    /// Invokes a Reward Redemption Event and allows replacing some components with custom data if more specific
    /// information has to be passed to the Client's event handlers.
    /// </summary>
    public static void InvokeFakeRedemptionEvent(
        this TwitchChatClient client,
        string? displayName = null,
        Guid? redemptionGuid = null
    )
    {
        OnRewardRedeemedArgs redemptionArgs =
            new() { DisplayName = displayName ?? "FakeName", RedemptionId = redemptionGuid ?? Guid.NewGuid() };

        TwitchChatClient.Instance.ManuallyInvokeRedemptionEvent(redemptionArgs);
    }

    private static TEventArgsType CreateSubscriberArgs<TSubscriberType, TEventArgsType>(
        string? displayName = null,
        string? userId = null,
        SubscriptionPlan? subscriptionPlan = null
    )
        where TSubscriberType : SubscriberBase
        where TEventArgsType : EventArgs, new()
    {
        // TODO: This fails, fix it
        SubscriberBaseBuilder builder = SubscriberBaseBuilder.Create();

        // Find out what type of the subscriber we have specified
        string subscriberTypeString = typeof(TSubscriberType).Name switch
        {
            nameof(Subscriber) => "Subscriber",
            nameof(ReSubscriber) => "ReSubscriber",
            nameof(PrimePaidSubscriber) => "PrimePaidSubscriber",
            _ => "SomeUnknownType",
        };

        builder
            .WithDisplayName(displayName ?? "FakeName")
            .WithUserId(userId ?? "FakeId")
            .WithSubscribtionPlan(subscriptionPlan ?? SubscriptionPlan.Tier1);

        TSubscriberType subscriber = (TSubscriberType)builder.Build();
        TEventArgsType subArgs = new();
        PropertyInfo? property = subArgs.GetType().GetProperty(subscriberTypeString);

        NullGuard.ThrowIfNull(property);
        property.SetValue(subArgs, subscriber, null);

        return subArgs;
    }
}
