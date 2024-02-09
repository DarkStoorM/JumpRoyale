using System;
using TwitchLib.Client.Events;
using Utils;

namespace TwitchChat;

/// <summary>
/// Selected properties of <c>OnMessageReceivedArgs</c>. Additionally, this exposes <c>IsPrivileged</c> field, which is
/// a combination of Broadcaster / Moderator / VIP / Subscriber.
/// </summary>
public class ChatMessageEventArgs : EventArgs
{
    public ChatMessageEventArgs(OnMessageReceivedArgs eventArgs)
    {
        NullGuard.ThrowIfNull(eventArgs);

        bool isPrivileged =
            eventArgs.ChatMessage.IsSubscriber
            || eventArgs.ChatMessage.IsModerator
            || eventArgs.ChatMessage.IsVip
            || eventArgs.ChatMessage.IsBroadcaster;

        Message = eventArgs.ChatMessage.Message;
        DisplayName = eventArgs.ChatMessage.DisplayName;
        UserId = eventArgs.ChatMessage.UserId;
        ColorHex = eventArgs.ChatMessage.ColorHex;
        IsPrivileged = isPrivileged;
    }

    public string Message { get; init; }

    public string DisplayName { get; init; }

    public string UserId { get; init; }

    public string ColorHex { get; init; }

    public bool IsPrivileged { get; init; }
}
