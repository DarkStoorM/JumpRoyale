using System.Text.Json;
using System.Text.Json.Serialization;
using Constants.Twitch;

namespace Tests.Mocks;

public class FakeTwitchConfig(string accessToken, string channelId, string channelName)
{
    [JsonPropertyName(TwitchConstants.ConfigAccessTokenIndex)]
    public string AccessToken { get; } = accessToken;

    [JsonPropertyName(TwitchConstants.ConfigChannelIdIndex)]
    public string ChannelId { get; } = channelId;

    [JsonPropertyName(TwitchConstants.ConfigChannelNameIndex)]
    public string ChannelName { get; } = channelName;

    public string Serialize()
    {
        return JsonSerializer.Serialize(this);
    }
}
