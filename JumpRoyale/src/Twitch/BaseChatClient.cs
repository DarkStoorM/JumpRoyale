using System;
using System.IO;
using Constants.Twitch;
using Microsoft.Extensions.Configuration;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using TwitchLib.PubSub;

namespace TwitchChat;

public class BaseChatClient
{
    private readonly ITwitchChatInitConfig _config;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseChatClient"/> class.
    /// </summary>
    /// <param name="initConfig">Path to the main Twitch config json file.</param>
    protected BaseChatClient(ITwitchChatInitConfig initConfig)
    {
        _config = initConfig;

        ConfigurationBuilder builder = new();

        // Secrets is the main config for AccessToken, this info should not be stored within the repository. For things
        // like channel name/id, we don't care
        builder.AddUserSecrets<TwitchChatClient>();

        // Add the main twitch config file
        AddJsonConfig(builder, _config.JsonConfigPath);

        // Loads a local configuration file, which is used for Development purposes; overrides the main configuration by
        // replacing the channel name/id the flag is used for testing purposes
        AddJsonConfig(builder, ResourcePaths.LocalTwitchConfig);

        Configuration = new(builder);
        TwitchClient = InitializeClient();
    }

    protected ChannelConfiguration Configuration { get; private set; }

    protected TwitchPubSub TwitchPubSub { get; private set; } = new();

    protected TwitchClient TwitchClient { get; private set; }

    protected void ConnectToTwitch()
    {
        TwitchPubSub.Connect();
        TwitchClient.Connect();
    }

    private TwitchClient InitializeClient()
    {
        (ConnectionCredentials credentials, ClientOptions options) = ConfigureClient();
        WebSocketClient customClient = new(options);
        TwitchClient client = new(customClient);

        client.Initialize(credentials, Configuration.ChannelName);

        return client;
    }

    private Tuple<ConnectionCredentials, ClientOptions> ConfigureClient()
    {
        ConnectionCredentials credentials = new(Configuration.ChannelName, Configuration.AccessToken);
        ClientOptions clientOptions =
            new()
            {
                MessagesAllowedInPeriod = TwitchConstants.MaximumMessages,
                ThrottlingPeriod = TimeSpan.FromSeconds(TwitchConstants.ThrottlingInSeconds),
            };

        return new(credentials, clientOptions);
    }

    /// <summary>
    /// Loads a Json configuration file, replacing any overlapping keys.
    /// </summary>
    /// <param name="builder">Configuration builder.</param>
    /// <param name="path">Path to the json file.</param>
    private void AddJsonConfig(ConfigurationBuilder builder, string path)
    {
        // Sometimes, we just don't need to load an additional configuration file, for example, when testing, so whether
        // the extra config file exists or not, we can just skip it without replacing the already loaded config.
        if (_config.SkipLocalConfig)
        {
            return;
        }

        builder.AddJsonFile($"{Directory.GetCurrentDirectory()}{path}");
    }
}
