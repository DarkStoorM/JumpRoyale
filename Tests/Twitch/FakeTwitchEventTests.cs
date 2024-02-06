using Twitch.Tests;
using TwitchChat;
using TwitchLib.Client.Enums;

namespace Tests.Twitch;

[TestFixture]
public class FakeTwitchEventTests : BaseTwitchTests
{
    private bool _testCase;
    private RewardRedemptionEventArgs _redemptionArgs;
    private ChatMessageEventArgs _chatMessageEventArgs;
    private SubscriberEventArgs _subscribeEventArgs;

    [SetUp]
    public new void SetUp()
    {
        _testCase = false;
        _redemptionArgs = null!;
        _chatMessageEventArgs = null!;
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
        TwitchChatClient.Instance.OnMessageEvent += MessageListener;

        TwitchChatClient.Instance.InvokeFakeMessageEvent();

        Assert.That(_testCase, Is.True);

        // Unsubscribe as cleanup
        TwitchChatClient.Instance.OnMessageEvent -= MessageListener;
    }

    [Test]
    public void CanInvokeRedemptionEvent()
    {
        TwitchChatClient.Instance.OnRedemptionEvent += RedemptionListener;

        TwitchChatClient.Instance.InvokeFakeRedemptionEvent();

        Assert.That(_testCase, Is.True);

        TwitchChatClient.Instance.OnRedemptionEvent -= RedemptionListener;
    }

    [Test]
    public void CanInvokeNewSubscriberEvent()
    {
        TwitchChatClient.Instance.OnSubscribeEvent += SubscriberListener;

        TwitchChatClient.Instance.InvokeFakeNewSubscriberEvent();

        Assert.That(_testCase, Is.True);

        TwitchChatClient.Instance.OnSubscribeEvent -= SubscriberListener;
    }

    [Test]
    public void CanInvokeReSubscriberEvent()
    {
        TwitchChatClient.Instance.OnSubscribeEvent += SubscriberListener;

        TwitchChatClient.Instance.InvokeFakeReSubscriberEvent();

        Assert.That(_testCase, Is.True);

        TwitchChatClient.Instance.OnSubscribeEvent -= SubscriberListener;
    }

    [Test]
    public void CanInvokePrimeSubscriberEvent()
    {
        TwitchChatClient.Instance.OnSubscribeEvent += SubscriberListener;

        TwitchChatClient.Instance.InvokeFakePrimeSubscriberEvent();

        Assert.That(_testCase, Is.True);

        TwitchChatClient.Instance.OnSubscribeEvent -= SubscriberListener;
    }

    [Test]
    public void CanGetDataFromFakeRedemptionInvocation()
    {
        TwitchChatClient.Instance.OnRedemptionEvent += RedemptionListener;

        Guid guid = Guid.NewGuid();
        TwitchChatClient.Instance.InvokeFakeRedemptionEvent("SomeGuy", guid);

        // Just assume that "SomeGuy" redeemed a reward of "guid"
        Assert.Multiple(() =>
        {
            Assert.That(_redemptionArgs.DisplayName, Is.EqualTo("SomeGuy"));
            Assert.That(_redemptionArgs.RedemptionId, Is.EqualTo(guid));
        });

        TwitchChatClient.Instance.OnRedemptionEvent -= RedemptionListener;
    }

    [Test]
    public void CanGetDataFromFakeMessageInvocation()
    {
        TwitchChatClient.Instance.OnMessageEvent += MessageListener;

        TwitchChatClient.Instance.InvokeFakeMessageEvent("FakeMessage", "FakeName", "FakeUserId", "AAAAAA");

        Assert.Multiple(() =>
        {
            Assert.That(_chatMessageEventArgs.Message, Is.EqualTo("FakeMessage"));
            Assert.That(_chatMessageEventArgs.DisplayName, Is.EqualTo("FakeName"));
            Assert.That(_chatMessageEventArgs.UserId, Is.EqualTo("FakeUserId"));
            Assert.That(_chatMessageEventArgs.ColorHex, Is.EqualTo("AAAAAA"));
        });

        TwitchChatClient.Instance.OnMessageEvent -= MessageListener;
    }

    [Test]
    public void CanGetDataFromFakeNewSubscriberEvent()
    {
        TwitchChatClient.Instance.OnSubscribeEvent += SubscriberListener;

        TwitchChatClient.Instance.InvokeFakeNewSubscriberEvent(
            "FakeName",
            "FakeUserId",
            "AAAAAA",
            SubscriptionPlan.Tier2
        );

        Assert.Multiple(() =>
        {
            Assert.That(_subscribeEventArgs.DisplayName, Is.EqualTo("FakeName"));
            Assert.That(_subscribeEventArgs.UserId, Is.EqualTo("FakeUserId"));
            Assert.That(_subscribeEventArgs.ColorHex, Is.EqualTo("AAAAAA"));
            Assert.That(_subscribeEventArgs.SubscriptionPlan, Is.EqualTo(SubscriptionPlan.Tier2));
        });

        TwitchChatClient.Instance.OnSubscribeEvent -= SubscriberListener;
    }

    [Test]
    public void CanGetDataFromFakeReSubscriberEvent()
    {
        TwitchChatClient.Instance.OnSubscribeEvent += SubscriberListener;

        TwitchChatClient.Instance.InvokeFakeReSubscriberEvent(
            "FakeName",
            "FakeUserId",
            "AAAAAA",
            SubscriptionPlan.Tier3
        );

        Assert.Multiple(() =>
        {
            Assert.That(_subscribeEventArgs.DisplayName, Is.EqualTo("FakeName"));
            Assert.That(_subscribeEventArgs.UserId, Is.EqualTo("FakeUserId"));
            Assert.That(_subscribeEventArgs.SubscriptionPlan, Is.EqualTo(SubscriptionPlan.Tier3));
        });

        TwitchChatClient.Instance.OnSubscribeEvent -= SubscriberListener;
    }

    [Test]
    public void CanGetDataFromFakePrimeSubscriberEvent()
    {
        TwitchChatClient.Instance.OnSubscribeEvent += SubscriberListener;

        TwitchChatClient.Instance.InvokeFakePrimeSubscriberEvent("FakeName", "FakeUserId", "AAAAAA");

        Assert.Multiple(() =>
        {
            Assert.That(_subscribeEventArgs.DisplayName, Is.EqualTo("FakeName"));
            Assert.That(_subscribeEventArgs.UserId, Is.EqualTo("FakeUserId"));
            Assert.That(_subscribeEventArgs.ColorHex, Is.EqualTo("AAAAAA"));
            Assert.That(_subscribeEventArgs.SubscriptionPlan, Is.EqualTo(SubscriptionPlan.Prime));
        });

        TwitchChatClient.Instance.OnSubscribeEvent -= SubscriberListener;
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
