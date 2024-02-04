using Tests.Mocks;
using TwitchChat;
using Utils.Exceptions;

namespace Tests.Twitch;

[TestFixture]
public class TwitchChatClientTests
{
    private readonly string _fullPathToTestConfig = $"{Directory.GetCurrentDirectory()}\\_testData\\config.json";
    private readonly string _pathToTestConfig = "\\_testData\\config.json";
    private readonly string _pathToLocalConfig = "\\_testData\\local.json";

    [SetUp]
    public void SetUp()
    {
        File.WriteAllText(_fullPathToTestConfig, CreateConfigFile());

        // This will only initialize the chat client if it was destroyed at some point in the tests
        Initialize();
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(_fullPathToTestConfig))
        {
            File.Delete(_fullPathToTestConfig);
        }
    }

    /// <summary>
    /// Since the singleton is initialized in the SetUp, we have to first destroy it and make sure it can throw an
    /// exception if it was uninitialized.
    /// </summary>
    [Test]
    public void ThrowsExceptionIfTwitchClientWasUninitialized()
    {
        // Just a sanity check, check if it was actually initialized
        Assert.DoesNotThrow(() =>
        {
            TwitchChatClient.Instance.GetType();
        });

        Assert.Throws<InvalidOperationException>(() =>
        {
            TwitchChatClient.Destroy();
            TwitchChatClient.Instance.GetType();
        });
    }

    /// <summary>
    /// This test will force current scenarios where a single field is missing by rewriting the config file.
    /// </summary>
    [Test]
    public void ThrowsIfConfigWasIncomplete()
    {
        File.WriteAllText(_fullPathToTestConfig, CreateConfigFile(accessToken: string.Empty));

        Assert.Throws<MissingTwitchAccessTokenException>(() =>
        {
            TwitchChatClient.Destroy();
            Initialize();
        });

        File.WriteAllText(_fullPathToTestConfig, CreateConfigFile(channelId: string.Empty));

        Assert.Throws<MissingTwitchChannelIdException>(() =>
        {
            TwitchChatClient.Destroy();
            Initialize();
        });

        File.WriteAllText(_fullPathToTestConfig, CreateConfigFile(channelName: string.Empty));

        Assert.Throws<MissingTwitchChannelNameException>(() =>
        {
            TwitchChatClient.Destroy();
            Initialize();
        });
    }

    /// <summary>
    /// This test makes sure the Twitch Chat Client will throw an exception when we request to load the local config
    /// file and it did not exist.
    /// </summary>
    [Test]
    public void ThrowsIfLocalConfigNotExistsButWasRequested()
    {
        Assert.Throws<FileNotFoundException>(() =>
        {
            TwitchChatClient.Destroy();
            Initialize(false);
        });
    }

    /// <summary>
    /// This test makes sure the Twitch Chat Client will throw an exception when we try to load a Main json config file
    /// that was presumably moved to a different path.
    /// </summary>
    [Test]
    public void ThrowsIfMainConfigNotExists()
    {
        File.Delete(_fullPathToTestConfig);

        Assert.Throws<FileNotFoundException>(() =>
        {
            TwitchChatClient.Destroy();
            Initialize(false);
        });
    }

    /// <summary>
    /// Initializes the TwitchChatClient with fake configuration file, omitting the connection to Twitch Services.
    /// </summary>
    /// <param name="skipLocalConfig">If true, omits loading the secondary, Local Json config file.</param>
    /// <param name="shouldConnectToTwitch">If true, forces the client to connect to Twitch. False by default.</param>
    private void Initialize(bool skipLocalConfig = true, bool shouldConnectToTwitch = false)
    {
        TwitchChatClient.Initialize(new(_pathToTestConfig, _pathToLocalConfig, skipLocalConfig, shouldConnectToTwitch));
    }

    private string CreateConfigFile(
        string accessToken = "FakeToken",
        string channelId = "FakeId",
        string channelName = "SomeFakeChannelName"
    )
    {
        return new FakeTwitchConfig(accessToken, channelId, channelName).Serialize();
    }
}
