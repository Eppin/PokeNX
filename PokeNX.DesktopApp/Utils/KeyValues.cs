namespace PokeNX.DesktopApp.Utils;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Models.Enums;
using Models;

public static class KeyValues
{
    public static Collection<KeyValue<ShinyFilter, string>> Shinies = new()
    {
        new KeyValue<ShinyFilter, string>(ShinyFilter.Any, "Any"),
        new KeyValue<ShinyFilter, string>(ShinyFilter.Star, "Star"),
        new KeyValue<ShinyFilter, string>(ShinyFilter.Square, "Square"),
        new KeyValue<ShinyFilter, string>(ShinyFilter.StarSquare, "Star/Square")
    };

    public static Collection<KeyValue<uint, string>> GenderRatio = new()
    {
        new KeyValue<uint, string>(255, "Genderless"),
        new KeyValue<uint, string>(127, "50% ♂ / 50% ♀"),
        new KeyValue<uint, string>(191, "25% ♂ / 75% ♀"),
        new KeyValue<uint, string>(63, "75% ♂ / 25% ♀"),
        new KeyValue<uint, string>(31, "87,5% ♂ / 12,5% ♀"),
        new KeyValue<uint, string>(0, "100% ♂"),
        new KeyValue<uint, string>(254, "100% ♀")
    };

    public static List<KeyValue<NatureFilter, string>> NaturesFilter = Enum.GetValues<NatureFilter>()
        .Select(n => new KeyValue<NatureFilter, string>(n, n.ToString()))
        .ToList();

    public static List<KeyValue<Nature, string>> Natures = Enum.GetValues<Nature>()
        .Select(n => new KeyValue<Nature, string>(n, n.ToString()))
        .ToList();

    public static List<KeyValue<Generator, string>> Generators = Enum.GetValues<Generator>()
        .Select(n => new KeyValue<Generator, string>(n, n.ToString()))
        .ToList();
}
