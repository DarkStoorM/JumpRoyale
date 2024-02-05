using Twitch.Tests;
using TwitchChat;
using Utils.Exceptions;

namespace Tests.Twitch;

[TestFixture]
public class TwitchChatClientTests : BaseTwitchTests
{
    [TearDown]
    public void TearDown()
    {
        if (File.Exists(FullPathToTestConfig))
        {
            File.Delete(FullPathToTestConfig);
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
        File.WriteAllText(FullPathToTestConfig, CreateConfigFile(accessToken: string.Empty));

        Assert.Throws<MissingTwitchAccessTokenException>(() =>
        {
            TwitchChatClient.Destroy();
            Initialize();
        });

        File.WriteAllText(FullPathToTestConfig, CreateConfigFile(channelId: string.Empty));

        Assert.Throws<MissingTwitchChannelIdException>(() =>
        {
            TwitchChatClient.Destroy();
            Initialize();
        });

        File.WriteAllText(FullPathToTestConfig, CreateConfigFile(channelName: string.Empty));

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
        File.Delete(FullPathToTestConfig);

        Assert.Throws<FileNotFoundException>(() =>
        {
            TwitchChatClient.Destroy();
            Initialize(false);
        });
    }
}
