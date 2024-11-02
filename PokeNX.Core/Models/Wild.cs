namespace PokeNX.Core.Models;

using Enums;

public class Wild
{
    public int Species { get; }

    public uint EC { get; }

    public uint PID { get; }

    public Nature Nature { get; }

    public Ability Ability { get; }

    public IEnumerable<int> IVs { get; }

    public Wild(int species, uint ec, uint pid, Nature nature, Ability ability, IEnumerable<int> ivs)
    {
        Species = species;
        EC = ec;
        PID = pid;
        Nature = nature;
        Ability = ability;
        IVs = ivs;
    }
}
