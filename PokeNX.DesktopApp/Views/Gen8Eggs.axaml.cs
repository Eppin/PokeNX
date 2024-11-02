namespace PokeNX.DesktopApp.Views
{
    using System;
    using System.Globalization;
    using Avalonia.Controls;
    using Avalonia.Data.Converters;
    using Avalonia.Interactivity;
    using Avalonia.Markup.Xaml;
    using Core.Generators;
    using ViewModels;
    using Inheritance = Core.Generators.Inheritance;
    using IVs = ViewModels.IVs;

    public partial class Gen8Eggs : UserControl
    {
        public Gen8Eggs()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            if (DataContext is not Gen8EggsViewModel d)
                return;

            if (!CompatibleParents(d.ParentA.Gender, d.ParentB.Gender))
            {
                d.ErrorText = "Parents aren't compatible!";

                return;
            }

            if (ReorderParents(d.ParentA.Gender, d.ParentB.Gender))
                (d.ParentA, d.ParentB) = (d.ParentB, d.ParentA);

            var compatibility = GetCompatibility(d.Compatibility, false); // TODO OvalCharm in a profile?

            var natureFilter = d.FilterStats.Nature == 0 ? null : new[] { (uint)d.FilterStats.Nature - 1 };

            uint genderRatio = (uint)d.FilterStats.GenderRatio switch
            {
                0 => 255,
                1 => 127,
                2 => 191,
                3 => 63,
                4 => 31,
                5 => 0,
                6 => 254,
                _ => throw new ArgumentOutOfRangeException()
            };

            var request = new Egg8Request
            {
                TrainerId = 64785, // TODO TID in a profile? 
                SecretId = 18176, // TODO SID in a profile?
                ParentA = d.ParentA,
                ParentB = d.ParentB,
                GenderRatio = genderRatio,
                Filter = new Filter
                {
                    Gender = d.FilterStats.Gender switch
                    {
                        0 => 255,
                        _ => (uint)d.FilterStats.Gender - 1
                    },
                    Natures = natureFilter,
                    Ability = d.FilterStats.Ability switch
                    {
                        0 => 255,
                        _ => (uint)d.FilterStats.Ability - 1
                    },
                    MinIVs = d.FilterStats.MinimumValues,
                    MaxIVs = d.FilterStats.MaximumValues,
                    Shiny = d.FilterStats.Shiny switch
                    {
                        0 => Shiny.All,
                        1 => Shiny.Star,
                        2 => Shiny.Square,
                        3 => Shiny.Any,
                        _ => throw new ArgumentOutOfRangeException()
                    }
                },
                IsMasuda = true,
                Compatibility = compatibility
            };

            var eggGen8 = new EggGenerator8(d.InitialAdvances, d.MaximumAdvances);

            if (d.Seed0 + d.Seed1 == 0)
            {
                d.ErrorText = "S0 and S1 cannot be 0!";

                return;
            }

            // Clear on success!
            d.ErrorText = string.Empty;

            var results = eggGen8.Generate(d.Seed0, d.Seed1, request);

            d.Persons.Clear();

            foreach (var r in results)
            {
                d.Persons.Add(new Person
                {
                    Advances = r.Advances,
                    Nature = r.Nature,
                    Shiny = r.Shiny,
                    IsShiny = r.Shiny > 0,
                    HP = new IVs(r.HP.Value, r.HP.Inheritance != null ? r.HP.Inheritance == Inheritance.A ? ViewModels.Inheritance.A : ViewModels.Inheritance.B : null),
                    Atk = new IVs(r.Atk.Value, r.Atk.Inheritance != null ? r.Atk.Inheritance == Inheritance.A ? ViewModels.Inheritance.A : ViewModels.Inheritance.B : null),
                    Def = new IVs(r.Def.Value, r.Def.Inheritance != null ? r.Def.Inheritance == Inheritance.A ? ViewModels.Inheritance.A : ViewModels.Inheritance.B : null),
                    SpA = new IVs(r.SpA.Value, r.SpA.Inheritance != null ? r.SpA.Inheritance == Inheritance.A ? ViewModels.Inheritance.A : ViewModels.Inheritance.B : null),
                    SpD = new IVs(r.SpD.Value, r.SpD.Inheritance != null ? r.SpD.Inheritance == Inheritance.A ? ViewModels.Inheritance.A : ViewModels.Inheritance.B : null),
                    Speed = new IVs(r.Speed.Value, r.Speed.Inheritance != null ? r.Speed.Inheritance == Inheritance.A ? ViewModels.Inheritance.A : ViewModels.Inheritance.B : null),
                    Gender = (Gender)r.Gender,
                    Ability = r.Ability,
                    PID = r.PID,
                    Seed = r.Seed
                });
            }

        }

        private static byte GetCompatibility(int compatibilityIndex, bool ovalCharm)
        {
            var compatibility = compatibilityIndex switch
            {
                0 => 20,
                1 => 50,
                2 => 70,
                _ => throw new ArgumentOutOfRangeException(nameof(compatibilityIndex), compatibilityIndex, null)
            };

            if (ovalCharm)
                compatibility = compatibility switch
                {
                    20 => 40,
                    50 => 80,
                    _ => 88
                };

            return (byte)compatibility;
        }

        private static bool CompatibleParents(int parent1, int parent2)
        {
            switch (parent1)
            {
                // Male/Female
                case 0 when parent2 == 1:
                case 1 when parent2 == 0:

                // Ditto/Female
                case 3 when parent2 == 1:
                case 1 when parent2 == 3:

                // Male/Ditto
                case 0 when parent2 == 3:
                case 3 when parent2 == 0:

                // Genderless/Ditto
                case 2 when parent2 == 3:
                case 3 when parent2 == 2:
                    return true;

                default:
                    return false;
            }
        }

        private static bool ReorderParents(int parent1, int parent2)
        {
            // Female/Male -> Male/Female
            var flag = parent1 == 1 && parent2 == 0;

            // Female/Ditto -> Ditto/Female
            flag |= parent1 == 1 && parent2 == 3;

            // Ditto/Male -> Male/Ditto
            flag |= parent1 == 3 && parent2 == 0;

            // Ditto/Genderless -> Genderless/Ditto
            flag |= parent1 == 3 && parent2 == 2;

            return flag;
        }
    }

    public class HexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string) || string.IsNullOrWhiteSpace(value.ToString()))
                return "0";

            try
            {
                var numeric = System.Convert.ToUInt64(value.ToString(), 16);

                return numeric.ToString("X16").ToUpper();
            }
            catch
            {
                return "0";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(ulong) || string.IsNullOrWhiteSpace(value.ToString()))
                return (ulong)0;

            try
            {
                return System.Convert.ToUInt64(value.ToString(), 16);
            }
            catch
            {
                return (ulong)0;
            }
        }
    }
}
