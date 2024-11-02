namespace PokeNX.DesktopApp.Models;

public class UseSeedMessage
{
    public ulong Seed0 { get; }

    public ulong Seed1 { get; }

    public UseSeedMessage(ulong seed0, ulong seed1)
    {
        Seed0 = seed0;
        Seed1 = seed1;
    }
}
