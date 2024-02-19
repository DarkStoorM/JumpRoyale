using JumpRoyale;
using JumpRoyale.Commands;
using Tests.Mocks;
using Tests.Twitch;
using TwitchChat;
using TwitchChat.Extensions;

namespace Tests.Game;

[TestFixture]
public class IntegrationTests : BaseTwitchTests
{
    private FakeTwitchChatter _fakeChatter;

    private PlayerData _fakePlayerData;

    private ChatMessageEventArgs _chatMessageEventArgs;

    [SetUp]
    public new void SetUp()
    {
        TwitchChatClient.Instance.OnTwitchMessageReceivedEvent += JoinCommandListener;

        _fakePlayerData = FakePlayerData.Make();
        _fakeChatter = new FakeTwitchChatter(string.Empty);

        PlayerStats.Initialize($"{TestContext.CurrentContext.WorkDirectory}\\_TestData\\data.json");
        PlayerStats.Instance.UpdatePlayer(_fakePlayerData);
        PlayerStats.Instance.SaveAllPlayers();

        _chatMessageEventArgs = null!;
    }

    [TearDown]
    public void TearDown()
    {
        TwitchChatClient.Instance.OnTwitchMessageReceivedEvent -= JoinCommandListener;
    }

    [Test]
    public void CanJoinTheGame()
    {
        _fakeChatter.Message = "join";

        // Assume the fake player sent "join" in the chat
        InvokeMessageEvent();

        Assert.Pass();
    }

    private void JoinCommandListener(object sender, ChatMessageEventArgs args)
    {
        _chatMessageEventArgs = args;

        ChatCommandHandler command = new(args.Message, args.UserId, args.DisplayName, args.ColorHex, true);
    }

    private void InvokeMessageEvent()
    {
        TwitchChatClient.Instance.InvokeFakeMessageEvent(
            _fakeChatter.Message,
            _fakeChatter.DisplayName,
            _fakeChatter.UserId,
            _fakeChatter.ColorHex
        );
    }
}
