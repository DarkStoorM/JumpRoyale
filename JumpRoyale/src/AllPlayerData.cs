using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace JumpRoyale;

public class AllPlayerData
{
    [JsonPropertyName("players")]
    public Dictionary<string, PlayerData> Players { get; } = [];
}
