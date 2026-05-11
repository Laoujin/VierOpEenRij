using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using ConnectFour.Engine;

namespace ConnectFour.Desktop.Converters;

public sealed class CellStateToBrushConverter : IValueConverter
{
    public static readonly CellStateToBrushConverter Instance = new();

    private static readonly IBrush BlueBrush = new RadialGradientBrush
    {
        GradientStops =
        {
            new GradientStop(Color.Parse("#7FB7FF"), 0),
            new GradientStop(Color.Parse("#1E5BC6"), 1)
        }
    };

    private static readonly IBrush RedBrush = new RadialGradientBrush
    {
        GradientStops =
        {
            new GradientStop(Color.Parse("#FF8A8A"), 0),
            new GradientStop(Color.Parse("#C61E1E"), 1)
        }
    };

    private static readonly IBrush EmptyBrush = new SolidColorBrush(Color.Parse("#1A1D24"));

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value switch
        {
            CellState.Blue => BlueBrush,
            CellState.Red => RedBrush,
            _ => EmptyBrush
        };

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}

public sealed class WinningCellBrushConverter : IValueConverter
{
    public static readonly WinningCellBrushConverter Instance = new();

    private static readonly IBrush Highlight = new SolidColorBrush(Color.Parse("#FFD93D"));
    private static readonly IBrush Transparent = Brushes.Transparent;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is true ? Highlight : Transparent;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
