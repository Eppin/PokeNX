namespace PokeNX.Core.Generators;

using Models;

public abstract class Generator8
{
    private protected readonly uint InitialAdvances;
    private protected readonly uint MaximumAdvances;

    protected Generator8(uint initialAdvances, uint maximumAdvances)
    {
        InitialAdvances = initialAdvances;
        MaximumAdvances = maximumAdvances;
    }

    private protected static bool Filter(Filter filter, GenerateResult result)
    {
        return filter.CompareShiny(result.Shiny) &&
               filter.CompareAbility(result.Ability) &&
               filter.CompareGender(result.Gender) &&
               filter.CompareNature(result.Nature) &&
               filter.CompareIVs(result.IVs);
    }
}
