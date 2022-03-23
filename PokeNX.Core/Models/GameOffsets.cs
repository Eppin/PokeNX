namespace PokeNX.Core.Models
{
    using System.Collections.Generic;
    using Enums;

    public class GameOffsets
    {
        public Game Game { get; }

        public IEnumerable<GameOffset> Offsets { get; }

        public GameOffsets(Game game, IEnumerable<GameOffset> offsets)
        {
            Game = game;
            Offsets = offsets;
        }
    }
}
