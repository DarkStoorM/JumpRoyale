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

    [SetUp]
    public new void SetUp()
    {
        PlayerStats.Destroy();

        TwitchChatClient.Instance.OnTwitchMessageReceivedEvent += JoinCommandListener;

        _fakeChatter = new FakeTwitchChatter(string.Empty);

        // Create some fake PlayerData that we will immediately store in the player stats file so we always have a
        // player ready to read from the file
        _fakePlayerData = FakePlayerData.Make(_fakeChatter.UserId, _fakeChatter.DisplayName);

        PlayerStats.Initialize($"{TestContext.CurrentContext.WorkDirectory}\\_TestData\\data.json");
        PlayerStats.Instance.UpdatePlayer(_fakePlayerData);
        PlayerStats.Instance.SaveAllPlayers();
    }

    [TearDown]
    public void TearDown()
    {
        TwitchChatClient.Instance.OnTwitchMessageReceivedEvent -= JoinCommandListener;
    }

    /// <summary>
    /// This test makes sure that when we send a message in the chat, we get our new Jumper stored.
    /// </summary>
    [Test]
    public void CanJoinTheGame()
    {
        // Assume the fake player sent "join" in the chat. Executing that, will use the created PlayerData from the
        // file, and the command handler will execute Join command, which should create and store a Jumper object. If
        // this passes the test, this can be repeated to check other inside cases of the Join command.
        InvokeMessageEvent("join");

        Jumper? jumper = PlayerStats.Instance.GetJumperByUserId(_fakeChatter.UserId);

        Assert.That(jumper, Is.Not.Null);
    }

    /// <summary>
    /// This test makes sure that we update the player's name inside the player stats when that player joins with a new
    /// name (changed through Twitch).
    /// </summary>
    [Test]
    public void CanUpdateChattersName()
    {
        _fakeChatter.DisplayName = "NewFake";

        InvokeMessageEvent("join");

        PlayerData? playerData = PlayerStats.Instance.GetPlayerById(_fakeChatter.UserId);

        Assert.That(playerData?.Name, Is.EqualTo("NewFake"));
    }

    private void JoinCommandListener(object sender, ChatMessageEventArgs args)
    {
        ChatCommandHandler command = new(args.Message, args.UserId, args.DisplayName, args.ColorHex, true);

        command.ProcessMessage();
    }

    private void InvokeMessageEvent(string messageToSend, bool isPrivileged = false)
    {
        TwitchChatClient.Instance.InvokeFakeMessageEvent(
            messageToSend,
            _fakeChatter.DisplayName,
            _fakeChatter.UserId,
            _fakeChatter.ColorHex,
            isPrivileged
        );
    }
}
