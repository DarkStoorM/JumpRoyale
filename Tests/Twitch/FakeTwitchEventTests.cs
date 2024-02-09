using Twitch.Tests;
using TwitchChat;
using TwitchChat.Extensions;
using TwitchLib.Client.Enums;

namespace Tests.Twitch;

[TestFixture]
public class FakeTwitchEventTests : BaseTwitchTests
{
    private bool _testCase;

    private BitsEventArgs _bitsEventArgs;
    private ChatMessageEventArgs _chatMessageEventArgs;
    private RewardRedemptionEventArgs _redemptionArgs;
    private SubscriberEventArgs _subscribeEventArgs;

    [SetUp]
    public new void SetUp()
    {
        _testCase = false;
        _bitsEventArgs = null!;
        _chatMessageEventArgs = null!;
        _redemptionArgs = null!;
        _subscribeEventArgs = null!;
    }

    /// <summary>
    /// This test makes sure that when we subscribe to exposed event and call it from manual invoke, we get our internal
    /// field modified as a result.
    /// </summary>
    [Test]
    public void CanInvokeMessageEvent()
    {
        // Sanity check 👀
        Assert.That(_testCase, Is.False);

        // Subscribe with a dummy method
        TwitchChatClient.Instance.OnTwitchMessageReceivedEvent += MessageListener;

        TwitchChatClient.Instance.InvokeFakeMessageEvent();

        Assert.That(_testCase, Is.True);

        // Unsubscribe as cleanup
        TwitchChatClient.Instance.OnTwitchMessageReceivedEvent -= MessageListener;
    }

    [Test]
    public void CanInvokeRedemptionEvent()
    {
        TwitchChatClient.Instance.OnTwitchRewardRedemptionEvent += RedemptionListener;

        TwitchChatClient.Instance.InvokeFakeRedemptionEvent();

        Assert.That(_testCase, Is.True);

        TwitchChatClient.Instance.OnTwitchRewardRedemptionEvent -= RedemptionListener;
    }

    [Test]
    public void CanInvokeNewSubscriberEvent()
    {
        TwitchChatClient.Instance.OnTwitchSubscriptionEvent += SubscriberListener;

        TwitchChatClient.Instance.InvokeFakeNewSubscriberEvent();

        Assert.That(_testCase, Is.True);

        TwitchChatClient.Instance.OnTwitchSubscriptionEvent -= SubscriberListener;
    }

    [Test]
    public void CanInvokeReSubscriberEvent()
    {
        TwitchChatClient.Instance.OnTwitchSubscriptionEvent += SubscriberListener;

        TwitchChatClient.Instance.InvokeFakeReSubscriberEvent();

        Assert.That(_testCase, Is.True);

        TwitchChatClient.Instance.OnTwitchSubscriptionEvent -= SubscriberListener;
    }

    [Test]
    public void CanInvokePrimeSubscriberEvent()
    {
        TwitchChatClient.Instance.OnTwitchSubscriptionEvent += SubscriberListener;

        TwitchChatClient.Instance.InvokeFakePrimeSubscriberEvent();

        Assert.That(_testCase, Is.True);

        TwitchChatClient.Instance.OnTwitchSubscriptionEvent -= SubscriberListener;
    }

    [Test]
    public void CanInvokeBitsEvent()
    {
        TwitchChatClient.Instance.OnTwitchBitsReceivedEvent += BitsCheerListener;

        TwitchChatClient.Instance.InvokeFakeBitsEvent();

        Assert.That(_testCase, Is.True);

        TwitchChatClient.Instance.OnTwitchBitsReceivedEvent -= BitsCheerListener;
    }

    [Test]
    public void CanGetDataFromFakeRedemptionInvocation()
    {
        TwitchChatClient.Instance.OnTwitchRewardRedemptionEvent += RedemptionListener;

        Guid guid = Guid.NewGuid();
        TwitchChatClient.Instance.InvokeFakeRedemptionEvent("SomeGuy", guid);

        // Just assume that "SomeGuy" redeemed a reward of "guid"
        Assert.Multiple(() =>
        {
            Assert.That(_redemptionArgs.DisplayName, Is.EqualTo("SomeGuy"));
            Assert.That(_redemptionArgs.RedemptionId, Is.EqualTo(guid));
        });

        TwitchChatClient.Instance.OnTwitchRewardRedemptionEvent -= RedemptionListener;

        Assert.Pass($"{_redemptionArgs.DisplayName} redeemed {_redemptionArgs.RedemptionId}");
    }

    [Test]
    public void CanGetDataFromFakeMessageInvocation()
    {
        TwitchChatClient.Instance.OnTwitchMessageReceivedEvent += MessageListener;

        TwitchChatClient.Instance.InvokeFakeMessageEvent("FakeMessage", "FakeName", "FakeUserId", "AAAAAA");

        // Make sure the Invoker gave us the same data
        Assert.Multiple(() =>
        {
            Assert.That(_chatMessageEventArgs.Message, Is.EqualTo("FakeMessage"));
            Assert.That(_chatMessageEventArgs.DisplayName, Is.EqualTo("FakeName"));
            Assert.That(_chatMessageEventArgs.UserId, Is.EqualTo("FakeUserId"));
            Assert.That(_chatMessageEventArgs.ColorHex, Is.EqualTo("AAAAAA"));
        });

        TwitchChatClient.Instance.OnTwitchMessageReceivedEvent -= MessageListener;

        Assert.Pass($"Message detected: {_chatMessageEventArgs.DisplayName} - {_chatMessageEventArgs.Message}");
    }

    [Test]
    public void CanGetDataFromFakeNewSubscriberEvent()
    {
        TwitchChatClient.Instance.OnTwitchSubscriptionEvent += SubscriberListener;

        TwitchChatClient.Instance.InvokeFakeNewSubscriberEvent(
            "FakeName",
            "FakeUserId",
            "AAAAAA",
            SubscriptionPlan.Tier2
        );

        // This test uses Tier2 sub to make sure the we did actually set the sub plan and didn't get Tier1 in return
        Assert.Multiple(() =>
        {
            Assert.That(_subscribeEventArgs.DisplayName, Is.EqualTo("FakeName"));
            Assert.That(_subscribeEventArgs.UserId, Is.EqualTo("FakeUserId"));
            Assert.That(_subscribeEventArgs.ColorHex, Is.EqualTo("AAAAAA"));
            Assert.That(_subscribeEventArgs.SubscriptionPlan, Is.EqualTo(SubscriptionPlan.Tier2));
        });

        TwitchChatClient.Instance.OnTwitchSubscriptionEvent -= SubscriberListener;

        Assert.Pass($"New sub: {_subscribeEventArgs.DisplayName} ({_subscribeEventArgs.SubscriptionPlan})");
    }

    [Test]
    public void CanGetDataFromFakeReSubscriberEvent()
    {
        TwitchChatClient.Instance.OnTwitchSubscriptionEvent += SubscriberListener;

        TwitchChatClient.Instance.InvokeFakeReSubscriberEvent(
            "FakeName",
            "FakeUserId",
            "AAAAAA",
            SubscriptionPlan.Tier3
        );

        // Similarly to New Sub, check with different tier just in case
        Assert.Multiple(() =>
        {
            Assert.That(_subscribeEventArgs.DisplayName, Is.EqualTo("FakeName"));
            Assert.That(_subscribeEventArgs.UserId, Is.EqualTo("FakeUserId"));
            Assert.That(_subscribeEventArgs.SubscriptionPlan, Is.EqualTo(SubscriptionPlan.Tier3));
        });

        TwitchChatClient.Instance.OnTwitchSubscriptionEvent -= SubscriberListener;

        Assert.Pass($"New Resub: {_subscribeEventArgs.DisplayName} ({_subscribeEventArgs.SubscriptionPlan})");
    }

    [Test]
    public void CanGetDataFromFakePrimeSubscriberEvent()
    {
        TwitchChatClient.Instance.OnTwitchSubscriptionEvent += SubscriberListener;

        TwitchChatClient.Instance.InvokeFakePrimeSubscriberEvent("FakeName", "FakeUserId", "AAAAAA");

        // Prime subs get a Prime sub plan by default inside the Invoker, so make sure we also get that
        Assert.Multiple(() =>
        {
            Assert.That(_subscribeEventArgs.DisplayName, Is.EqualTo("FakeName"));
            Assert.That(_subscribeEventArgs.UserId, Is.EqualTo("FakeUserId"));
            Assert.That(_subscribeEventArgs.ColorHex, Is.EqualTo("AAAAAA"));
            Assert.That(_subscribeEventArgs.SubscriptionPlan, Is.EqualTo(SubscriptionPlan.Prime));
        });

        TwitchChatClient.Instance.OnTwitchSubscriptionEvent -= SubscriberListener;

        Assert.Pass($"New Prime sub: {_subscribeEventArgs.DisplayName}");
    }

    [Test]
    public void CanGetDataFromFakeBitsEvent()
    {
        TwitchChatClient.Instance.OnTwitchBitsReceivedEvent += BitsCheerListener;

        TwitchChatClient.Instance.InvokeFakeBitsEvent(666, "SomeFakeId");

        Assert.Multiple(() =>
        {
            Assert.That(_bitsEventArgs.BitsAmount, Is.EqualTo(666));
            Assert.That(_bitsEventArgs.UserId, Is.EqualTo("SomeFakeId"));
        });

        TwitchChatClient.Instance.OnTwitchBitsReceivedEvent -= BitsCheerListener;

        Assert.Pass($"{_bitsEventArgs.UserId} cheered: {_bitsEventArgs.BitsAmount}");
    }

    private void BitsCheerListener(object sender, BitsEventArgs eventArgs)
    {
        _testCase = true;
        _bitsEventArgs = eventArgs;
    }

    private void MessageListener(object sender, ChatMessageEventArgs eventArgs)
    {
        _testCase = true;
        _chatMessageEventArgs = eventArgs;
    }

    private void RedemptionListener(object sender, RewardRedemptionEventArgs eventArgs)
    {
        _testCase = true;
        _redemptionArgs = eventArgs;
    }

    private void SubscriberListener(object sender, SubscriberEventArgs eventArgs)
    {
        _testCase = true;
        _subscribeEventArgs = eventArgs;
    }
}
