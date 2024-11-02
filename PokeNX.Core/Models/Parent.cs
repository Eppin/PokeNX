namespace PokeNX.Core.Models
{
    using Enums;

    public class Parent
    {
        public byte Ability { get; set; }

        public Nature Nature { get; set; }

        public EggHeldItem? HeldItem { get; set; }

        public byte[] IVs { get; set; } = new byte[6];
    }
}