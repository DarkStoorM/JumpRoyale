using System;
using Constants.Messages;
using TwitchLib.Client.Events;
using TwitchLib.PubSub.Events;
using Utils;

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

        TwitchClient.OnMessageReceived += HandleMessageReceived;
        TwitchClient.OnNewSubscriber += HandleNewSubscription;
        TwitchClient.OnReSubscriber += HandleReSubscription;
        TwitchClient.OnPrimePaidSubscriber += HandlePrimeSubscription;
        TwitchPubSub.OnRewardRedeemed += HandleRewardRedeemed;

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
    /// Event fired when this client gets notified by the Pub Sub about new reward redeems
    /// </summary>
    public event EventHandler<RewardRedemptionEventArgs>? OnRedemptionEvent;

    public event EventHandler<SubscriberEventArgs>? OnSubscribeEvent;

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

    /// <summary>
    /// Initializes the TwitchChatClient with provided configuration. If empty instance of <c>TwitchChatInitConfig</c>
    /// was provided, it will be initialized with default configs. See <see cref="TwitchChatInitConfig"/> for more
    /// information.
    /// </summary>
    /// <param name="initConfig">Configuration object with Json config file paths.</param>
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

    #region Streamer/Testing zone // These have to be public, can't invoke from the outside of the class

    /// <summary>
    /// Allows invoking the Chat Message Event without relying on Twitch services. See <see
    /// cref="TwitchChatClientExtensions"/> for more information on how to invoke this event.
    /// </summary>
    public void ManuallyInvokeMessageEvent(OnMessageReceivedArgs eventArgs)
    {
        NullGuard.ThrowIfNull(eventArgs);

        HandleMessageReceived(this, eventArgs);
    }

    /// <summary>
    /// Allows invoking the Reward Redemption Event without relying on Twitch services. See <see
    /// cref="TwitchChatClientExtensions"/> for more information on how to invoke this event.
    /// </summary>
    public void ManuallyInvokeRedemptionEvent(OnRewardRedeemedArgs eventArgs)
    {
        NullGuard.ThrowIfNull(eventArgs);

        HandleRewardRedeemed(this, eventArgs);
    }

    /// <summary>
    /// Allows invoking the New Subscriber Event without relying on Twitch services. See <see
    /// cref="TwitchChatClientExtensions"/> for more information on how to invoke this event.
    /// </summary>
    public void ManuallyInvokeNewSubscriberEvent(OnNewSubscriberArgs eventArgs)
    {
        HandleNewSubscription(this, eventArgs);
    }

    /// <summary>
    /// Allows invoking the ReSubscriber Event without relying on Twitch services. See <see
    /// cref="TwitchChatClientExtensions"/> for more information on how to invoke this event.
    /// </summary>
    public void ManuallyInvokeReSubscriberEvent(OnReSubscriberArgs eventArgs)
    {
        HandleReSubscription(this, eventArgs);
    }

    /// <summary>
    /// Allows invoking the Prime Subscriber Event without relying on Twitch services. See <see
    /// cref="TwitchChatClientExtensions"/> for more information on how to invoke this event.
    /// </summary>
    public void ManuallyInvokePrimeSubscriberEvent(OnPrimePaidSubscriberArgs eventArgs)
    {
        HandlePrimeSubscription(this, eventArgs);
    }
    #endregion

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

    #region Event handlers responsible for Invoking

    private void HandleMessageReceived(object sender, OnMessageReceivedArgs e)
    {
        OnMessageEvent?.Invoke(this, new ChatMessageEventArgs(e));
    }

    private void HandleNewSubscription(object sender, OnNewSubscriberArgs e)
    {
        OnSubscribeEvent?.Invoke(this, new SubscriberEventArgs(e));
    }

    private void HandleReSubscription(object sender, OnReSubscriberArgs e)
    {
        OnSubscribeEvent?.Invoke(this, new SubscriberEventArgs(e));
    }

    private void HandlePrimeSubscription(object sender, OnPrimePaidSubscriberArgs e)
    {
        OnSubscribeEvent?.Invoke(this, new SubscriberEventArgs(e));
    }

    private void HandleRewardRedeemed(object sender, OnRewardRedeemedArgs e)
    {
        Console.WriteLine(TwitchMessages.OnRewardRedeemMessage.ReplaceInTemplate(e.DisplayName, e.RedemptionId));

        OnRedemptionEvent?.Invoke(this, new RewardRedemptionEventArgs(e));
    }

    #endregion
}
