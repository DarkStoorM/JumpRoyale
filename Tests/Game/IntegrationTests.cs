using JumpRoyale;
using JumpRoyale.Commands;
using Tests.Mocks;
using Tests.Twitch;
using TwitchChat;
using TwitchChat.Extensions;

namespace Tests.Game;

/// <summary>
/// Provides a set of test cases that "simulate" the regular flow of how the commands should be handled in the game.
/// Every new case, when testing a new feature or command, should start with "join" command invocation to have the
/// player stored, because he's required to be present before he can execute any command other than <c>join</c>.
/// </summary>
[TestFixture]
public class IntegrationTests : BaseTwitchTests
{
    private FakeTwitchChatter _fakeChatter;
    private PlayerData _playerData;
    private Jumper _jumper;

    [SetUp]
    public new void SetUp()
    {
        PlayerStats.Destroy();

        TwitchChatClient.Instance.OnTwitchMessageReceivedEvent += JoinCommandListener;

        _fakeChatter = new FakeTwitchChatter();

        // Create some fake PlayerData that we will immediately store in the player stats file so we always have a
        // player ready to read from the file
        PlayerData dummyPlayerData = FakePlayerData.Make(_fakeChatter.UserId, _fakeChatter.DisplayName);

        PlayerStats.Initialize($"{TestContext.CurrentContext.WorkDirectory}\\_TestData\\data.json");
        PlayerStats.Instance.UpdatePlayer(dummyPlayerData);
        PlayerStats.Instance.SaveAllPlayers();

        _playerData = null!;
        _jumper = null!;
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

        Assert.That(_jumper, Is.Not.Null);
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

        Assert.That(_playerData.Name, Is.EqualTo("NewFake"));
    }

    /// <summary>
    /// This test makes sure that when a previously privileged user joins the game, but he is not a subscriber anymore
    /// (or privileged in general), his name color choice is ignored and reverted to default color.
    /// </summary>
    [Test]
    public void CanRevertToDefaultNameWhenUnprivileged()
    {
        InvokeMessageEvent("join");

        Assert.Multiple(() =>
        {
            Assert.That(_playerData.PlayerNameColor.ToLower(), Is.EqualTo("ffffff"));
            Assert.That(_jumper.PlayerData.PlayerNameColor.ToLower(), Is.EqualTo("ffffff"));
        });
    }

    /// <summary>
    /// This is just a sanity check test to make sure that we can change the player data directly from PlayerData object
    /// or from the Jumper. The purpose of this is that in some situations we would like to affect PlayerData from the
    /// Jumper object, e.g. on Update or when an event is raised within Jumper's context.
    /// </summary>
    [Test]
    public void CanMutateReference()
    {
        InvokeMessageEvent("join");

        // Set it to something stupid to avoid randomness collision. It's still 16 million combinations, should we care?
        _jumper.PlayerData.GlowColor = "PPPPPP";

        Assert.That(_playerData.GlowColor, Is.EqualTo(_jumper.PlayerData.GlowColor));
    }

    /// <summary>
    /// This test makes sure that we can handle correctly the old save data, which held <c>null</c> as Name/Glow Color.
    /// </summary>
    [Test]
    public void CanDefaultFromEmptyColors()
    {
        GetPlayerData();

        // Force saving a player with `null` as namecolor just for this test case
        _playerData.PlayerNameColor = null!;
        _playerData.GlowColor = null!;

        RefreshPlayerData();
        InvokeMessageEvent("join", true);

        Assert.Multiple(() =>
        {
            Assert.That(_playerData.PlayerNameColor, Is.Not.Null);
            Assert.That(_playerData.GlowColor, Is.Not.Null);
        });
    }

    /// <summary>
    /// This test makes sure that when joining for the first time, privileged users get their Twitch chat color assigned
    /// to the name/glow color and not the default color.
    /// </summary>
    [Test]
    public void CanDefaultToTwitchColors()
    {
        GetPlayerData();

        // Assume those exist in the save data, doesn't matter they are invalid in this case
        _playerData.PlayerNameColor = "fakecolor";
        _playerData.GlowColor = "fakecolor";

        RefreshPlayerData();
        InvokeMessageEvent("join", true);

        Assert.Multiple(() =>
        {
            Assert.That(_playerData.PlayerNameColor, Is.EqualTo("fakecolor"));
            Assert.That(_playerData.GlowColor, Is.EqualTo("fakecolor"));
        });
    }

    /// <summary>
    /// This test makes sure that when unprivileged user joins the game, his name color is changed to default.
    /// </summary>
    [Test]
    public void CanChangeToDefaultColorWhenUnprivileged()
    {
        GetPlayerData();

        // Force a color choice
        _playerData.PlayerNameColor = "fakecolor";

        RefreshPlayerData();
        InvokeMessageEvent("join");

        Assert.That(_playerData.PlayerNameColor.ToLower(), Is.EqualTo("ffffff"));
    }

    /// <summary>
    /// This test makes sure that when an unprivileged user tries to execute a NameColor command, his color does not get
    /// changed, which is a result of CommandHandler ignoring the incoming request.
    /// </summary>
    [Test]
    public void UnprivilegedUserCantExecuteNameColorCommand()
    {
        GetPlayerData();

        string previousColor = _playerData.PlayerNameColor;

        InvokeMessageEvent("join");
        InvokeMessageEvent("namecolor bada55");

        Assert.That(_playerData.PlayerNameColor, Is.EqualTo(previousColor));
    }

    /// <summary>
    /// This test makes sure that when an unprivileged user tries to execute a Glow command, his color does not get
    /// changed, which is a result of CommandHandler ignoring the incoming request.
    /// </summary>
    [Test]
    public void UnprivilegedUserCantExecuteGlowCommand()
    {
        GetPlayerData();

        string previousColor = _playerData.GlowColor;

        InvokeMessageEvent("join");
        InvokeMessageEvent("glow bada55");

        Assert.That(_playerData.GlowColor, Is.EqualTo(previousColor));
    }

    [Test]
    public void PrivilegedUserCanChangeNameColor()
    {
        InvokeMessageEvent("join", true);
        InvokeMessageEvent("namecolor bada55", true);

        Assert.That(_playerData.PlayerNameColor.ToLower(), Is.EqualTo("bada55"));
    }

    [Test]
    public void PrivilegedUserCanChangeGlowColor()
    {
        InvokeMessageEvent("join", true);
        InvokeMessageEvent("glow bada55", true);

        Assert.That(_playerData.GlowColor.ToLower(), Is.EqualTo("bada55"));
    }

    /// <summary>
    /// Updates the PlayerData in the Player Stats collection, Serializes all players and reloads them from file.
    /// </summary>
    private void RefreshPlayerData()
    {
        PlayerStats.Instance.UpdatePlayer(_playerData!);
        PlayerStats.Instance.SaveAllPlayers();

        PlayerStats.Instance.ClearPlayers();
        PlayerStats.Instance.LoadPlayerData();
    }

    private void JoinCommandListener(object sender, ChatMessageEventArgs args)
    {
        ChatCommandHandler command = new(args.Message, args.UserId, args.DisplayName, args.ColorHex, args.IsPrivileged);

        command.ProcessMessage();
    }

    /// <summary>
    /// Invokes a fake message event as fake user, allows specifying the sent message and changing the privileged status
    /// ot the fake user.
    /// </summary>
    /// <param name="messageToSend">Message to send to the event - literal chat message content, e.g. "join".</param>
    /// <param name="isPrivileged">Current privilege status of the fake user. <c>false</c> by default.</param>
    private void InvokeMessageEvent(string messageToSend, bool isPrivileged = false)
    {
        TwitchChatClient.Instance.InvokeFakeMessageEvent(
            messageToSend,
            _fakeChatter.DisplayName,
            _fakeChatter.UserId,
            _fakeChatter.ColorHex,
            isPrivileged
        );

        GetPlayerData();
        GetJumper();
    }

    /// <summary>
    /// Shortcut for retrieving the Jumper for the internally created/stored FakeChatter. Has to be called if we want to
    /// force-update before accessing.
    /// </summary>
    private void GetJumper()
    {
        _jumper = PlayerStats.Instance.GetJumperByUserId(_fakeChatter.UserId) ?? throw new Exception("No player data");
    }

    /// <summary>
    /// Shortcut for retrieving the PlayerData for the internally created/stored FakeChatter. Has to be called if we
    /// want to force-update before accessing.
    /// </summary>
    private void GetPlayerData()
    {
        _playerData = PlayerStats.Instance.GetPlayerById(_fakeChatter.UserId) ?? throw new Exception("No jumper");
    }
}
