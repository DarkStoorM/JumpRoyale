using System;
using System.Drawing;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Builders;
using TwitchLib.PubSub.Events;

namespace TwitchChat.Extensions;

public static class TwitchChatClientExtensions
{
    /// <summary>
    /// Allows faking a Message Received event internally for testing purposes. Allows modifying the properties for fake
    /// user customization, if needed.
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

        TwitchChatClient.Instance.FakeMessageEvent(messageArgs);
    }

    /// <summary>
    /// Allows faking a New Subscriber event internally for testing purposes. Allows customizing some of the subscriber
    /// data, e.g. Subscription Plan, if needed.
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

        TwitchChatClient.Instance.FakeNewSubscriberEvent(subArgs);
    }

    /// <summary>
    /// See: <see cref="InvokeFakeNewSubscriberEvent"/>.
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

        TwitchChatClient.Instance.FakeReSubscriberEvent(resubArgs);
    }

    /// <summary>
    /// Allows faking a Prime subscription event internally for testing purposes. Unlike New/Resub events, this will
    /// have SubscriptionPlan set to Prime by default.
    /// </summary>
    public static void InvokeFakePrimeSubscriberEvent(
        this TwitchChatClient client,
        string? displayName = null,
        string? userId = null,
        string? colorHex = null
    )
    {
        // Sadly, there is no Prime Sub Builder, so we have to call the constructor directly...
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

        TwitchChatClient.Instance.FakePrimeSubscriberEvent(primeArgs);
    }

    /// <summary>
    /// Allows faking a reward redemption event internally for testing purposes.
    /// </summary>
    public static void InvokeFakeRedemptionEvent(
        this TwitchChatClient client,
        string? displayName = null,
        Guid? redemptionGuid = null
    )
    {
        OnRewardRedeemedArgs redemptionArgs =
            new() { DisplayName = displayName ?? "FakeName", RedemptionId = redemptionGuid ?? Guid.NewGuid() };

        TwitchChatClient.Instance.FakeRedemptionEvent(redemptionArgs);
    }

    /// <summary>
    /// Allows faking the Cheering (Twitch Bits) event for testing purposes. This will store <c>100</c> bits by default.
    /// </summary>
    public static void InvokeFakeBitsEvent(this TwitchChatClient client, int? bitsAmount = null, string? userId = null)
    {
        OnBitsReceivedArgs bitsArgs = new() { BitsUsed = bitsAmount ?? 100, UserId = userId ?? "FakeId" };

        TwitchChatClient.Instance.FakeBitsEvent(bitsArgs);
    }
}
