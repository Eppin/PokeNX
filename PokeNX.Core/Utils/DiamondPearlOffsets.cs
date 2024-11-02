namespace PokeNX.Core.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using Models;
using Models.Enums;
using static Models.Enums.Version;

public static class DiamondPearlOffsets
{
    private static readonly IDictionary<string, GameOffsets> GameOffsets = new Dictionary<string, GameOffsets>
    {
        {
            "0100000011D90000",
            new GameOffsets(Game.BrilliantDiamond, new[]
            {
                new GameOffset("D9E96FB92878E345", V111, 0x4C49098, "[main+4F8CCD0]"),
                new GameOffset("1B5215DF918BA04B", V112, 0x4E60170, "[main+4F8CCD0]"),
                new GameOffset("BC259F7EE8E79A49", V113, 0x4E853F0, "[main+4FB2050]"),
                new GameOffset("35B9D8779B195141", V120, 0x4E61DD0, "[main+4F8E750]"),
                new GameOffset("94CEAE325C205C4B", V130, 0x4C90330, "[main+4FD43D0]")
            })
        },
        {
            "010018E011D92000",
            new GameOffsets(Game.ShiningPearl, new[]
            {
                new GameOffset("3C70CAE153DF0B4F", V111, 0x4E60170, "[main+4F8CCD0]"),
                new GameOffset("5D3A3B56321FFD4C", V112, 0x4E60170, "[main+4F8CCD0]"),
                new GameOffset("046D130F0873314A", V113, 0x4E853F0, "[main+4FB2050]"),
                new GameOffset("D75246EC33C2F64B", V120, 0x4E61DD0, "[main+4F8E750]"),
                new GameOffset("38F59CBDA2EB9C44", V130, 0x4EA7408, "[main+4FD43D0]")
            })
        }
    };

    public static (Game game, GameOffset) GetGameOffset(string titleId, string buildId)
    {
        if (!GameOffsets.TryGetValue(titleId, out var gameOffsets))
            throw new ArgumentOutOfRangeException(nameof(titleId), titleId, $"Only compatible with {nameof(Game.BrilliantDiamond)} or {nameof(Game.ShiningPearl)}");

        var gameOffset = gameOffsets.Offsets.SingleOrDefault(g => g.BuildId.Equals(buildId, StringComparison.OrdinalIgnoreCase));

        if (gameOffset == null)
            throw new ArgumentOutOfRangeException(nameof(buildId), buildId, $"Unsupported build detected for game {gameOffsets.Game}");

        return (gameOffsets.Game, gameOffset);
    }
}
