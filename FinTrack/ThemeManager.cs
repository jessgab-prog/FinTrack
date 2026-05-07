using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace FinTrack;

public static class ThemeManager
{
    public static bool IsDark { get; private set; } = false;
    public static event Action<bool>? ThemeChanged;

    public static void Apply(bool isDark)
    {
        IsDark = isDark;
        ThemeChanged?.Invoke(isDark);
    }

    public static SolidColorBrush PageBackground => Brush(IsDark ? "#1A1A1E" : "#F5F5F0");
    public static SolidColorBrush CardBackground => Brush(IsDark ? "#25252B" : "#FFFFFF");
    public static SolidColorBrush TextPrimary => Brush(IsDark ? "#F0F0EC" : "#1A1A1A");
    public static SolidColorBrush TextSecondary => Brush(IsDark ? "#9A9A94" : "#666666");
    public static SolidColorBrush BorderColor => Brush(IsDark ? "#3A3A42" : "#E8E8E2");
    public static SolidColorBrush InputBackground => Brush(IsDark ? "#2E2E36" : "#FFFFFF");
    public static SolidColorBrush AlternateRow => Brush(IsDark ? "#2A2A32" : "#FAFAF8");
    public static SolidColorBrush GridBackground => Brush(IsDark ? "#25252B" : "#FFFFFF");
    public static SolidColorBrush HeaderForeground => Brush(IsDark ? "#9A9A94" : "#888888");
    public static SolidColorBrush RowForeground => Brush(IsDark ? "#F0F0EC" : "#1A1A1A");

    public static void ApplyToDataGrid(DataGrid grid)
    {
        grid.Background = GridBackground;
        grid.Foreground = RowForeground;
        grid.RowBackground = GridBackground;
        grid.AlternatingRowBackground = AlternateRow;
        grid.BorderBrush = BorderColor;
        grid.HorizontalGridLinesBrush = BorderColor;
        grid.VerticalGridLinesBrush = Brushes.Transparent;

        var headerStyle = new Style(typeof(DataGridColumnHeader));
        headerStyle.Setters.Add(new Setter(DataGridColumnHeader.BackgroundProperty, GridBackground));
        headerStyle.Setters.Add(new Setter(DataGridColumnHeader.ForegroundProperty, HeaderForeground));
        headerStyle.Setters.Add(new Setter(DataGridColumnHeader.FontWeightProperty, FontWeights.SemiBold));
        headerStyle.Setters.Add(new Setter(DataGridColumnHeader.PaddingProperty, new Thickness(12, 8, 12, 8)));
        headerStyle.Setters.Add(new Setter(DataGridColumnHeader.BorderBrushProperty, BorderColor));
        headerStyle.Setters.Add(new Setter(DataGridColumnHeader.BorderThicknessProperty, new Thickness(0, 0, 0, 1)));
        grid.ColumnHeaderStyle = headerStyle;

        var rowStyle = new Style(typeof(DataGridRow));
        rowStyle.Setters.Add(new Setter(DataGridRow.ForegroundProperty, RowForeground));
        rowStyle.Setters.Add(new Setter(DataGridRow.BackgroundProperty, GridBackground));
        grid.RowStyle = rowStyle;
    }

    private static SolidColorBrush Brush(string hex) =>
        new((Color)ColorConverter.ConvertFromString(hex));
}