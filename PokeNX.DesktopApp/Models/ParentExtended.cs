namespace PokeNX.DesktopApp.Models
{
    using Core.Models;
    using Core.Models.Enums;

    public class ParentExtended : Parent
    {
        public byte HP { get => IVs[0]; set => IVs[0] = value; }

        public byte Atk { get => IVs[1]; set => IVs[1] = value; }

        public byte Def { get => IVs[2]; set => IVs[2] = value; }

        public byte SpA { get => IVs[3]; set => IVs[3] = value; }

        public byte SpD { get => IVs[4]; set => IVs[4] = value; }

        public byte Speed { get => IVs[5]; set => IVs[5] = value; }

        public new int Ability { get => base.Ability; set => base.Ability = (byte)value; }

        public int Gender { get; set; }

        public new int Nature { get => (int)base.Nature; set => base.Nature = (Nature)value; }

        public new int HeldItem { get => (int)base.HeldItem.GetValueOrDefault(); set => base.HeldItem = (EggHeldItem)value; }
    }
}
