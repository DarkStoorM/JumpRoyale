using System;
using Constants.Messages;
using TwitchLib.Client.Events;
using TwitchLib.PubSub.Events;

namespace TwitchChat;

public class TwitchChatClient : BaseChatClient
{
    private static readonly object _lock = new();

    private static TwitchChatClient? _instance;

    private TwitchChatClient(TwitchChatInitConfig initConfig)
        : base(initConfig)
    {
        TwitchClient.OnConnected += OnConnected;
        TwitchClient.OnJoinedChannel += OnJoinedChannel;
        TwitchPubSub.OnPubSubServiceConnected += OnPubSubServiceConnected;

        TwitchClient.OnMessageReceived += OnMessageReceived;
        TwitchPubSub.OnRewardRedeemed += OnRewardRedeemed;

        if (InitConfig.AutomaticallyConnectToTwitch)
        {
            ConnectToTwitch();
        }
    }

    /// <summary>
    /// Event fired when Twitch Client receives a new chat message
    /// </summary>
    public event EventHandler<ChatMessageEventArgs>? OnMessageEvent;

    /// <summary>
    /// Event fired when Twitch Client
    /// </summary>
    public event EventHandler<RewardRedemptionEventArgs>? OnRedemptionEvent;

    public static TwitchChatClient Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance is null)
                {
                    throw new InvalidOperationException("Call Initialize() before using TwitchChatClient.");
                }
            }

            return _instance;
        }
    }

    public static void Initialize(TwitchChatInitConfig initConfig)
    {
        lock (_lock)
        {
            _instance ??= new TwitchChatClient(initConfig);
        }
    }

    /// <summary>
    /// Destroys the currently existing singleton for refreshing purposes, e.g. Initializing with different configs in
    /// in separate scenes or during testing.
    /// </summary>
    public static void Destroy()
    {
        _instance = null;
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

    private void OnRewardRedeemed(object sender, OnRewardRedeemedArgs e)
    {
        Console.WriteLine(TwitchMessages.OnRewardRedeemMessage.ReplaceInTemplate(e.DisplayName, e.RedemptionId));

        OnRedemptionEvent?.Invoke(this, new RewardRedemptionEventArgs(e));
    }
}
