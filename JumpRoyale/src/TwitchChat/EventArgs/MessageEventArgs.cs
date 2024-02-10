using System;
using JumpRoyale.Utils;
using TwitchLib.Client.Events;

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

    public string Message { get; }

    public string DisplayName { get; }

    public string UserId { get; }

    public string ColorHex { get; }

    public bool IsPrivileged { get; }
}
