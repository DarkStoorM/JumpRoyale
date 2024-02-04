using System;
using System.Diagnostics.CodeAnalysis;
using Constants.Twitch;
using Microsoft.Extensions.Configuration;
using Utils.Exceptions;

namespace TwitchChat;

/// <summary>
/// Provides the configuration keys for Twitch channel.
/// </summary>
public class ChannelConfiguration
{
    private readonly IConfigurationRoot _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChannelConfiguration"/> class.
    /// </summary>
    /// <param name="config">Builder template with already included data.</param>
    public ChannelConfiguration([NotNull] ConfigurationBuilder config)
    {
        _configuration = config.Build();

        // Make sure everything is set. Accessing any of these when null/empty will throw an exception
        AccessToken.AsSpan();
        ChannelName.AsSpan();
        ChannelId.AsSpan();
    }

    /// <summary>
    /// Access Token of the user authenticating to Twitch services.
    /// </summary>
    public string AccessToken
    {
        get => TryGetPropertyFromConfig(TwitchConstants.ConfigAccessTokenIndex);
    }

    /// <summary>
    /// Twitch User Name of the account connecting to the Twitch services.
    /// </summary>
    public string ChannelName
    {
        get => TryGetPropertyFromConfig(TwitchConstants.ConfigChannelNameIndex);
    }

    /// <summary>
    /// Channel ID this client will use to listen for various events.
    /// </summary>
    public string ChannelId
    {
        get => TryGetPropertyFromConfig(TwitchConstants.ConfigChannelIdIndex);
    }

    private string TryGetPropertyFromConfig(string index)
    {
        // When trying to access an unset configuration key, throw an appropriate exception type with custom message. If
        // more configuration keys are added and we don't have a specific exception for that, we will throw a generic
        // exception. This is only for readability purposes messages are set through TwitchConstants.
        string? value = _configuration[index];

        // Shortcut for selecting an exception to throw if the config key was null or empty
        return value is null || value.Length == 0
            ? throw index switch
            {
                TwitchConstants.ConfigAccessTokenIndex => new MissingTwitchAccessTokenException(),
                TwitchConstants.ConfigChannelNameIndex => new MissingTwitchChannelNameException(),
                TwitchConstants.ConfigChannelIdIndex => new MissingTwitchChannelIdException(),
                _ => new System.Exception($"Missing user-secrets index: ({index})"),
            }
            : value;
    }
}
