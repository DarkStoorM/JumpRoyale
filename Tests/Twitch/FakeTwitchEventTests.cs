using Twitch.Tests;
using TwitchChat;

namespace Tests.Twitch;

[TestFixture]
public class FakeTwitchEventTests : BaseTwitchTests
{
    private bool _testCase;
    private RewardRedemptionEventArgs _redemptionArgs;
    private ChatMessageEventArgs _chatMessageEventArgs;

    [SetUp]
    public new void SetUp()
    {
        _testCase = false;
        _redemptionArgs = null!;
        _chatMessageEventArgs = null!;
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

    /// <summary>
    /// This test makes sure that when we subscribe to exposed event and call it from manual invoke, we get our internal
    /// field modified as a result.
    /// </summary>
    [Test]
    public void CanInvokeRedemptionEvent()
    {
        TwitchChatClient.Instance.OnRedemptionEvent += RedemptionListener;

        TwitchChatClient.Instance.InvokeFakeRedemptionEvent();

        Assert.That(_testCase, Is.True);

        TwitchChatClient.Instance.OnRedemptionEvent -= RedemptionListener;
    }

    /// <summary>
    /// This test makes sure that we can get the same fake data we passed into the fake invocation.
    /// </summary>
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

    /// <summary>
    /// This test makes sure that we can get the same fake data we passed into the fake invocation.
    /// </summary>
    [Test]
    public void CanGetDataFromFakeMessageInvocation()
    {
        TwitchChatClient.Instance.OnMessageEvent += MessageListener;

        TwitchChatClient.Instance.InvokeFakeMessageEvent("FakeMessage", "FakeName", "FakeUserId", "FakeColor");

        Assert.Multiple(() =>
        {
            Assert.That(_chatMessageEventArgs.Message, Is.EqualTo("FakeMessage"));
            Assert.That(_chatMessageEventArgs.DisplayName, Is.EqualTo("FakeName"));
            Assert.That(_chatMessageEventArgs.UserId, Is.EqualTo("FakeUserId"));
            Assert.That(_chatMessageEventArgs.ColorHex, Is.EqualTo("FakeColor"));
        });

        TwitchChatClient.Instance.OnMessageEvent -= MessageListener;
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
}
