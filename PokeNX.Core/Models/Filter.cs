namespace PokeNX.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Enums;

    public class Filter
    {
        public GenderFilter Gender { get; set; }

        public AbilityFilter Ability { get; set; }

        public ShinyFilter Shiny { get; set; }

        public byte[]? MinIVs { get; set; }

        public byte[]? MaxIVs { get; set; }

        public IEnumerable<NatureFilter> Natures { get; set; }

        public bool CompareShiny(Shiny shiny)
        {
            return Shiny switch
            {
                ShinyFilter.Any => true,
                ShinyFilter.Star => shiny is Enums.Shiny.Star,
                ShinyFilter.Square => shiny is Enums.Shiny.Square,
                ShinyFilter.StarSquare => shiny is Enums.Shiny.Star or Enums.Shiny.Square,
                _ => throw new ArgumentOutOfRangeException(nameof(Shiny), Shiny, null)
            };
        }

        public bool CompareAbility(Ability ability) => Ability == AbilityFilter.Any || (byte)Ability == (byte)ability;

        public bool CompareGender(Gender gender) => Gender == GenderFilter.Any || (byte)Gender == (byte)gender;

        public bool CompareNature(Nature nature) => Natures.Contains(NatureFilter.Any) || Natures.Any(n => (byte)n == (byte)nature);

        public bool CompareIVs(byte[] ivs)
        {
            if (MinIVs == null || MaxIVs == null)
                return true;

            for (var i = 0; i < 6; i++)
            {
                var iv = ivs[i];

                if (iv < MinIVs[i] || iv > MaxIVs[i])
                    return false;
            }

            return true;
        }
    }
}