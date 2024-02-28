using JumpRoyale;
using JumpRoyale.Utils;

namespace Tests.Mocks;

public static class FakePlayerData
{
    public static PlayerData Make(string? userId = null, string? name = null, string? colorHex = null)
    {
        PlayerData fakePlayer =
            new(colorHex ?? Rng.RandomHex(), Rng.RandomInt(), Rng.RandomHex(), userId ?? Rng.RandomInt().ToString())
            {
                Num1stPlaceWins = Rng.RandomInt(),
                Num2ndPlaceWins = Rng.RandomInt(),
                Num3rdPlaceWins = Rng.RandomInt(),
                NumJumps = Rng.RandomInt(),
                NumPlays = Rng.RandomInt(),
                HighestWinStreak = Rng.RandomInt(),
                WinStreak = Rng.RandomInt(),
                Name = name ?? Path.GetRandomFileName(), /* looks like: "zewrsrzg.rfp" */
            };

        return fakePlayer;
    }
}
