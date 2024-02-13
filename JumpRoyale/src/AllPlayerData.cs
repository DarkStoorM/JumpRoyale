using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace JumpRoyale;

public class AllPlayerData
{
    [JsonPropertyName("players")]
#pragma warning disable CA2227 // Collection properties should be read only
    public Dictionary<string, PlayerData> Players { get; set; } = [];
#pragma warning restore CA2227 // Collection properties should be read only
}
