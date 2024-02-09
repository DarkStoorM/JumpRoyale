using System;
using TwitchChat.Messages;
using TwitchLib.Client.Events;
using TwitchLib.Communication.Events;
using TwitchLib.PubSub.Events;

namespace TwitchChat;

/// <summary>
/// Twitch Client/PubSub singleton with exposed events. Call TwitchChatClient.Initialize(new()) once.
/// <para><c>Initialize()</c> takes in an instance of <c>TwitchChatInitConfig</c>. For more information, see <see
/// cref="TwitchChatInitConfig"/>.</para>
/// </summary>
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

        TwitchClient.OnError += OnClientError;
        TwitchClient.OnConnectionError += OnConnectionError;
        TwitchPubSub.OnPubSubServiceError += OnPubSubError;

        if (InitConfig.AutomaticallyConnectToTwitch)
        {
            ConnectToTwitch();
        }
    }

    public event EventHandler<BitsEventArgs>? OnTwitchBitsReceivedEvent;

    public event EventHandler<ChatMessageEventArgs>? OnTwitchMessageReceivedEvent;

    public event EventHandler<RewardRedemptionEventArgs>? OnTwitchRewardRedemptionEvent;

    public event EventHandler<SubscriberEventArgs>? OnTwitchSubscriptionEvent;

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

    // Note: See TwitchChatClientExtensions on how to invoke the following events
    #region Streamer/Testing zone // These have to be public, can't invoke from the outside of the class

    public void FakeBitsEvent(OnBitsReceivedArgs args) => HandleBitsReceived(this, args);

    public void FakeMessageEvent(OnMessageReceivedArgs args) => HandleMessageReceived(this, args);

    public void FakeNewSubscriberEvent(OnNewSubscriberArgs args) => HandleNewSubscription(this, args);

    public void FakePrimeSubscriberEvent(OnPrimePaidSubscriberArgs args) => HandlePrimeSubscription(this, args);

    public void FakeRedemptionEvent(OnRewardRedeemedArgs args) => HandleRewardRedeemed(this, args);

    public void FakeReSubscriberEvent(OnReSubscriberArgs args) => HandleReSubscription(this, args);

    #endregion

    #region Event handlers responsible for Invoking

    private void HandleBitsReceived(object sender, OnBitsReceivedArgs args) =>
        OnTwitchBitsReceivedEvent?.Invoke(this, new BitsEventArgs(args));

    private void HandleMessageReceived(object sender, OnMessageReceivedArgs args) =>
        OnTwitchMessageReceivedEvent?.Invoke(this, new ChatMessageEventArgs(args));

    private void HandleNewSubscription(object sender, OnNewSubscriberArgs args) =>
        OnTwitchSubscriptionEvent?.Invoke(this, new SubscriberEventArgs(args));

    private void HandlePrimeSubscription(object sender, OnPrimePaidSubscriberArgs args) =>
        OnTwitchSubscriptionEvent?.Invoke(this, new SubscriberEventArgs(args));

    private void HandleReSubscription(object sender, OnReSubscriberArgs args) =>
        OnTwitchSubscriptionEvent?.Invoke(this, new SubscriberEventArgs(args));

    private void HandleRewardRedeemed(object sender, OnRewardRedeemedArgs args) =>
        OnTwitchRewardRedemptionEvent?.Invoke(this, new RewardRedemptionEventArgs(args));

    #endregion

    #region Error handlers

    private void OnClientError(object sender, OnErrorEventArgs args)
    {
        // Consider using a logger for any console logs or errors
        Console.WriteLine(args.Exception);
    }

    private void OnConnectionError(object sender, OnConnectionErrorArgs args)
    {
        // Consider using a logger for any console logs or errors
        Console.WriteLine(args.Error);
    }

    private void OnPubSubError(object sender, OnPubSubServiceErrorArgs args)
    {
        // Consider using a logger for any console logs or errors
        Console.WriteLine(args.Exception);
    }

    #endregion

    private void OnConnected(object sender, OnConnectedArgs args)
    {
        // Consider using a logger for any console logs or errors
        Console.WriteLine(TwitchMessages.OnClientConnectedMessage);
    }

    private void OnJoinedChannel(object sender, OnJoinedChannelArgs args)
    {
        // Consider using a logger for any console logs or errors
        Console.WriteLine(TwitchMessages.OnChannelJoinMessage);
    }

    private void OnPubSubServiceConnected(object sender, EventArgs e)
    {
        // Consider using a logger for any console logs or errors
        Console.WriteLine(TwitchMessages.OnPubSubConnected);

        TwitchPubSub.ListenToBitsEvents(Configuration.ChannelId);
        TwitchPubSub.ListenToRewards(Configuration.ChannelId);
        TwitchPubSub.ListenToSubscriptions(Configuration.ChannelId);

        // Is this where __Broadcaster__ token goes for Bits/new Rewards events? 🤔
        //                     \/
        TwitchPubSub.SendTopics();
    }
}
