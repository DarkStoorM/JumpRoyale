using System;
using Constants.Messages;
using TwitchLib.Client.Events;
using TwitchLib.PubSub.Events;

namespace TwitchChat;

public class TwitchChatClient : BaseChatClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TwitchChatClient"/> class.
    /// </summary>
    public TwitchChatClient(ITwitchChatInitConfig initConfig)
        : base(initConfig)
    {
        TwitchClient.OnConnected += OnConnected;
        TwitchClient.OnJoinedChannel += OnJoinedChannel;
        TwitchPubSub.OnPubSubServiceConnected += OnPubSubServiceConnected;

        TwitchClient.OnMessageReceived += OnMessageReceived;
        TwitchPubSub.OnRewardRedeemed += OnRewardRedeemed;

        ConnectToTwitch();
    }

    /// <summary>
    /// Event fired when Twitch Client receives a new chat message
    /// </summary>
    public event EventHandler<ChatMessageEventArgs>? OnMessageEvent;

    /// <summary>
    /// Event fired when Twitch Client
    /// </summary>
    public event EventHandler<OnRewardRedeemedArgs>? OnRedemptionEvent;

    private void OnRewardRedeemed(object sender, OnRewardRedeemedArgs e)
    {
        Console.WriteLine(TwitchMessages.OnRewardRedeemMessage.ReplaceInTemplate(e.DisplayName, e.RedemptionId));

        OnRedemptionEvent?.Invoke(this, e);
    }

    private void OnConnected(object sender, OnConnectedArgs e)
    {
        Console.WriteLine(TwitchMessages.OnClientConnectedMessage);
    }

    private void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
    {
        Console.WriteLine(TwitchMessages.OnChannelJoinMessage);
    }

    private void OnPubSubServiceConnected(object sender, EventArgs e)
    {
        Console.WriteLine(TwitchMessages.OnPubSubConnected);

        TwitchPubSub.ListenToRewards(Configuration.ChannelId);
        TwitchPubSub.SendTopics();
    }

    private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
    {
        OnMessageEvent?.Invoke(this, new ChatMessageEventArgs(e));
    }
}
