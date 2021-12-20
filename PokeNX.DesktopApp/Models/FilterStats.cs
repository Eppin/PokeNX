namespace PokeNX.DesktopApp.Models
{
    using Utils;

    public class FilterStats
    {
        public Values HP { get; set; } = new();

        public Values Atk { get; set; } = new();

        public Values Def { get; set; } = new();

        public Values SpA { get; set; } = new();

        public Values SpD { get; set; } = new();

        public Values Speed { get; set; } = new();

        public byte[] MinimumValues => new[] { (byte)HP.Minimum, (byte)Atk.Minimum, (byte)Def.Minimum, (byte)SpA.Minimum, (byte)SpD.Minimum, (byte)Speed.Minimum };

        public byte[] MaximumValues => new[] { (byte)HP.Maximum, (byte)Atk.Maximum, (byte)Def.Maximum, (byte)SpA.Maximum, (byte)SpD.Maximum, (byte)Speed.Maximum };

        private int _gender;
        public int Gender
        {
            get => _gender == -1 ? 0 : _gender;
            set => _gender = value;
        }

        private int _genderRatio;
        public int GenderRatio
        {
            get => _genderRatio == -1 ? 0 : _genderRatio;
            set => _genderRatio = value;
        }

        private int _ability;
        public int Ability
        {
            get => _ability == -1 ? 0 : _ability;
            set => _ability = value;
        }

        // Select last item (Any)
        private static readonly int DefaultAny = KeyValues.NaturesFilter.Count - 1;

        private int _nature = DefaultAny;
        public int Nature
        {
            get => _nature == -1 ? DefaultAny : _nature;
            set => _nature = value;
        }

        private int _shiny;
        public int Shiny
        {
            get => _shiny == -1 ? 0 : _shiny;
            set => _shiny = value;
        }
    }
}
