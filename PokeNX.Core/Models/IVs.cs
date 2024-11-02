namespace PokeNX.Core.Models
{
    using Enums;

    public class IVs
    {
        public byte Value { get; set; }

        public Inheritance? Inheritance { get; set; }

        public bool InheritanceVisible => Inheritance != null;

        public IVs(byte value, Inheritance? inheritance = null)
        {
            Value = value;
            Inheritance = inheritance;
        }
    }
}