using System;
using TwitchLib.Client.Events;
using Utils;

namespace TwitchChat;

/// <summary>
/// Selected event arguments from OnMessageReceivedArgs.
/// </summary>
public class ChatMessageEventArgs : EventArgs
{
    public ChatMessageEventArgs(OnMessageReceivedArgs eventArgs)
    {
        NullGuard.ThrowIfNull<ArgumentNullException>(eventArgs);

        bool isPrivileged =
            eventArgs.ChatMessage.IsSubscriber
            || eventArgs.ChatMessage.IsModerator
            || eventArgs.ChatMessage.IsVip
            || eventArgs.ChatMessage.IsBroadcaster;

        Message = eventArgs.ChatMessage.Message;
        SenderName = eventArgs.ChatMessage.DisplayName;
        SenderId = eventArgs.ChatMessage.UserId;
        HexColor = eventArgs.ChatMessage.ColorHex;
        IsPrivileged = isPrivileged;
    }

    public string Message { get; init; }

    public string SenderName { get; init; }

    public string SenderId { get; init; }

    public string HexColor { get; init; }

    public bool IsPrivileged { get; init; }
}
