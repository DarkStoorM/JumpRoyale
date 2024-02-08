using System;
using System.Drawing;
using TwitchChat;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Builders;
using TwitchLib.PubSub.Events;

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
        string? colorHex = null,
        SubscriptionPlan? subscriptionPlan = null
    )
    {
        SubscriberBuilder builder = SubscriberBuilder.Create();

        builder
            .WithDisplayName(displayName ?? "FakeName")
            .WithUserId(userId ?? "FakeId")
            .WithColorHex(colorHex ?? "FFFFFF")
            .WithSubscribtionPlan(subscriptionPlan ?? SubscriptionPlan.Tier1);

        OnNewSubscriberArgs subArgs = new() { Subscriber = (Subscriber)builder.Build() };

        TwitchChatClient.Instance.ManuallyInvokeNewSubscriberEvent(subArgs);
    }

    /// <summary>
    /// Invokes a ReSubscriber Event and allows replacing some components with custom data if more specific
    /// information has to be passed to the Client's event handlers.
    /// </summary>
    public static void InvokeFakeReSubscriberEvent(
        this TwitchChatClient client,
        string? displayName = null,
        string? userId = null,
        string? colorHex = null,
        SubscriptionPlan? subscriptionPlan = null
    )
    {
        ReSubscriberBuilder builder = ReSubscriberBuilder.Create();

        builder
            .WithDisplayName(displayName ?? "FakeName")
            .WithColorHex(colorHex ?? "FFFFFF")
            .WithUserId(userId ?? "FakeId")
            .WithSubscribtionPlan(subscriptionPlan ?? SubscriptionPlan.Tier1);

        OnReSubscriberArgs resubArgs = new() { ReSubscriber = (ReSubscriber)builder.Build() };

        TwitchChatClient.Instance.ManuallyInvokeReSubscriberEvent(resubArgs);
    }

    /// <summary>
    /// Invokes a ReSubscriber Event and allows replacing some components with custom data if more specific
    /// information has to be passed to the Client's event handlers.
    /// </summary>
    public static void InvokeFakePrimeSubscriberEvent(
        this TwitchChatClient client,
        string? displayName = null,
        string? userId = null,
        string? colorHex = null
    )
    {
        // Sadly, there is no Prime Sub Builder...
        PrimePaidSubscriber prime =
            new(
                [],
                [],
                colorHex ?? "FFFFFF",
                Color.AliceBlue,
                displayName ?? "FakeName",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                false,
                string.Empty,
                string.Empty,
                SubscriptionPlan.Prime,
                string.Empty,
                string.Empty,
                userId ?? "FakeId",
                false,
                false,
                false,
                false,
                string.Empty,
                UserType.Viewer,
                string.Empty,
                string.Empty,
                0
            );

        OnPrimePaidSubscriberArgs primeArgs = new() { PrimePaidSubscriber = prime };
        TwitchChatClient.Instance.ManuallyInvokePrimeSubscriberEvent(primeArgs);
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

    public static void InvokeFakeBitsEvent(this TwitchChatClient client, int? bitsAmount = null, string? userId = null)
    {
        OnBitsReceivedArgs bitsArgs = new() { BitsUsed = bitsAmount ?? 100, UserId = userId ?? "FakeId" };

        TwitchChatClient.Instance.ManuallyInvokeBitsEvent(bitsArgs);
    }
}
