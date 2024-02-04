using Tests.Mocks;
using TwitchChat;

namespace Tests.Twitch;

[TestFixture]
public class TwitchChatClientTests
{
    private readonly string _testConfigPath = $"{Directory.GetCurrentDirectory()}\\_testData\\config.json";

    [SetUp]
    public void SetUp()
    {
        File.WriteAllText(_testConfigPath, CreateConfigFile());

        // This will only initialize the chat client if it was destroyed at some point in the tests
        Initialize();
    }

    [TearDown]
    public void OneTimeTearDown()
    {
        if (File.Exists(_testConfigPath))
        {
            File.Delete(_testConfigPath);
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

        TwitchChatClient.Destroy();

        Assert.Throws<InvalidOperationException>(() =>
        {
            TwitchChatClient.Instance.GetType();
        });
    }

    /// <summary>
    /// This test will force current scenarios where a single field is missing by rewriting the config file.
    /// </summary>
    [Test]
    public void ThrowsIfConfigWasIncomplete()
    {
        File.WriteAllText(_testConfigPath, CreateConfigFile(channelId: string.Empty));

        Assert.Throws<MissingTwitchChannelIdException>(() =>
        {
            TwitchChatClient.Destroy();
            Initialize();
        });
    }

    private void Initialize()
    {
        TwitchChatClient.Initialize(new("\\_testData\\config.json", true, false));
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
