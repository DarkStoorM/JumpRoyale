using System;
using TwitchChat;
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
        libMessageBuilder.WithDisplayName(displayName ?? "FakeName");
        libMessageBuilder.WithUserId(userId ?? "FakeId");
        libMessageBuilder.WithColorHex(colorHex ?? "FakeColor");

        ChatMessage chatMessage = messageBuilder.WithTwitchLibMessage(libMessageBuilder).Build();
        OnMessageReceivedArgs messageArgs = new() { ChatMessage = chatMessage };

        TwitchChatClient.Instance.ManuallyInvokeMessageEvent(messageArgs);
    }

    /// <summary>
    /// Invokes a Reward Redemption Message Event and allows replacing some components with custom data if more specific
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
}
