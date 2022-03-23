namespace PokeNX.Core.Models;

using Enums;

public class Wild8Request
{
    public ushort TrainerId { get; set; }

    public ushort SecretId { get; set; }

    public uint GenderRatio { get; set; }

    public Encounter Encounter { get; set; }

    public Lead Lead { get; set; }

    public Filter Filter { get; set; }
}
