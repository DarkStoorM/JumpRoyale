using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using TwitchChat.Constants;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using TwitchLib.PubSub;

namespace TwitchChat;

public class BaseChatClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseChatClient"/> class.
    /// </summary>
    /// <param name="initConfig">Initialization config object.</param>
    protected BaseChatClient(TwitchChatInitConfig initConfig)
    {
        InitConfig = initConfig;

        ConfigurationBuilder builder = new();

        // Secrets is the main config for AccessToken, this info should not be stored within the repository. For things
        // like channel name/id, we don't care
        builder.AddUserSecrets<TwitchChatClient>();

        // Add the main twitch config file
        AddJsonConfig(builder, InitConfig.JsonConfigPath);

        // Loads a local configuration file, which is used for Development purposes; overrides the main configuration by
        // replacing the channel name/id the flag is used for testing purposes
        AddJsonConfig(builder, TwitchConfigPaths.LocalTwitchConfig, InitConfig.SkipLocalConfig);

        Configuration = new(builder);
        TwitchClient = InitializeClient();
    }

    public TwitchClient TwitchClient { get; }

    public TwitchPubSub TwitchPubSub { get; } = new();

    protected ChannelConfiguration Configuration { get; }

    protected TwitchChatInitConfig InitConfig { get; }

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

    /// <summary>
    /// Returns Twitch connection credentials with client options used to connect to Twitch services.
    /// </summary>
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
    /// <param name="skipLocalConfig">If true, omits loading the Local configuration file for Twitch.</param>
    private void AddJsonConfig(ConfigurationBuilder builder, string path, bool skipLocalConfig = false)
    {
        string pathToConfig = $"{Directory.GetCurrentDirectory()}{path}";

        // Sometimes, we just don't need to load an additional configuration file, for example, when testing, so whether
        // the extra config file exists or not, we can just skip it without replacing the already loaded config.
        if (skipLocalConfig)
        {
            return;
        }

        if (!File.Exists(pathToConfig))
        {
            throw new FileNotFoundException($"The requested config file was not found: {pathToConfig}");
        }

        builder.AddJsonFile(pathToConfig);
    }
}
