using Tests.Mocks;
using TwitchChat;

namespace Tests.Twitch;

public class BaseTwitchTests
{
    protected string FullPathToTestConfig { get; } = $"{Directory.GetCurrentDirectory()}\\_testData\\config.json";

    protected string PathToTestConfig { get; } = "\\_testData\\config.json";

    protected string PathToLocalConfig { get; } = "\\_testData\\local.json";

    [SetUp]
    public void SetUp()
    {
        File.WriteAllText(FullPathToTestConfig, CreateConfigFile());

        // This will only initialize the chat client if it was destroyed at some point in the tests
        Initialize();
    }

    /// <summary>
    /// Destroys the Twitch Chat Client so other tests can prepare a fresh init.
    /// </summary>
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        TwitchChatClient.Destroy();
    }

    /// <summary>
    /// Initializes the TwitchChatClient with fake configuration file, omitting the connection to Twitch Services.
    /// </summary>
    /// <param name="skipLocalConfig">If true, omits loading the secondary, Local Json config file.</param>
    /// <param name="shouldConnectToTwitch">If true, forces the client to connect to Twitch. False by default.</param>
    protected void Initialize(bool skipLocalConfig = true, bool shouldConnectToTwitch = false)
    {
        TwitchChatClient.Initialize(new(PathToTestConfig, PathToLocalConfig, skipLocalConfig, shouldConnectToTwitch));
    }

    protected string CreateConfigFile(
        string accessToken = "FakeToken",
        string channelId = "FakeId",
        string channelName = "SomeFakeChannelName"
    )
    {
        return new FakeTwitchConfig(accessToken, channelId, channelName).Serialize();
    }
}
