using JumpRoyale.Utils;

namespace Tests.Mocks;

public class FakeTwitchChatter(string? displayName = null, string? userId = null, string? colorHex = null)
{
    public string ColorHex { get; set; } = colorHex ?? Rng.RandomHex();

    public string DisplayName { get; set; } = displayName ?? Path.GetRandomFileName();

    public string UserId { get; set; } = userId ?? Rng.RandomInt().ToString();
}
