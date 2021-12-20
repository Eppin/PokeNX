namespace PokeNX.Core.Models
{
    using Generators;

    public class Egg8Request
    {
        public ushort TrainerId { get; set; }

        public ushort SecretId { get; set; }

        public uint GenderRatio { get; set; }

        public bool IsMasuda { get; set; }

        public bool IsShinyCharm { get; set; }

        public bool IsVolbeatNidoran { get; set; }

        public byte Compatibility { get; set; }

        public Filter Filter { get; set; }

        public Parent ParentA { get; set; }

        public Parent ParentB { get; set; }
    }
}
