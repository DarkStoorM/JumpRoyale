using Constants.Twitch;
using TwitchChat;

namespace Tests.Twitch;

[TestFixture]
public class TwitchChatClientTests
{
    private readonly string _testConfigPath = $"{Directory.GetCurrentDirectory()}\\_testData\\config.json";

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        File.WriteAllText(
            _testConfigPath,
            $"{{\"{TwitchConstants.ConfigChannelIdIndex}\": null, \"{TwitchConstants.ConfigChannelNameIndex}\": \"FakeChannelName\" }}"
        );
    }

    [SetUp]
    public void SetUp()
    {
        // This will only initialize the chat client if it was destroyed at some point in the tests
        Initialize();
    }

    [OneTimeTearDown]
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

    [Test]
    public void ThrowsIfConfigWasIncomplete()
    {
        // Failing
        Assert.Throws<MissingTwitchChannelIdException>(() =>
        {
            TwitchChatClient.Destroy();
            Initialize();
            TwitchChatClient.Instance.GetType();
        });
    }

    private void Initialize()
    {
        TwitchChatClient.Initialize(new("\\_testData\\config.json", true));
    }
}
