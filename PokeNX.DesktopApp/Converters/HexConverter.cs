namespace PokeNX.DesktopApp.Converters;

using System;
using System.Globalization;
using Avalonia.Data.Converters;

public class HexConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType != typeof(string) || string.IsNullOrWhiteSpace(value.ToString()))
            return "0";

        try
        {
            var numeric = System.Convert.ToUInt64(value.ToString(), 10);

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
