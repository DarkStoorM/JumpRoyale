using JumpRoyale.Utils;

namespace Tests.Mocks;

public class FakeTwitchChatter(
    string message,
    string? displayName = null,
    string? userId = null,
    string? colorHex = null
)
{
    public string ColorHex { get; } = colorHex ?? Rng.RandomHex();

    public string DisplayName { get; } = displayName ?? Path.GetRandomFileName();

    public string Message { get; set; } = message;

    public string UserId { get; } = userId ?? Rng.RandomInt().ToString();
}
