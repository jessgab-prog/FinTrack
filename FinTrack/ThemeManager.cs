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

    // Core brushes
    public static SolidColorBrush PageBackground => Brush(IsDark ? "#1A1A1E" : "#F5F5F0");
    public static SolidColorBrush CardBackground => Brush(IsDark ? "#25252B" : "#FFFFFF");
    public static SolidColorBrush TextPrimary => Brush(IsDark ? "#F0F0EC" : "#1A1A1A");
    public static SolidColorBrush TextSecondary => Brush(IsDark ? "#9A9A94" : "#666666");
    public static SolidColorBrush TextMuted => Brush(IsDark ? "#6A6A64" : "#AAAAAA");
    public static SolidColorBrush BorderColor => Brush(IsDark ? "#3A3A42" : "#E8E8E2");
    public static SolidColorBrush InputBackground => Brush(IsDark ? "#2E2E36" : "#FFFFFF");
    public static SolidColorBrush InputForeground => Brush(IsDark ? "#F0F0EC" : "#1A1A1A");
    public static SolidColorBrush AlternateRow => Brush(IsDark ? "#2A2A32" : "#FAFAF8");
    public static SolidColorBrush GridBackground => Brush(IsDark ? "#25252B" : "#FFFFFF");
    public static SolidColorBrush HeaderForeground => Brush(IsDark ? "#9A9A94" : "#888888");
    public static SolidColorBrush RowForeground => Brush(IsDark ? "#F0F0EC" : "#1A1A1A");
    public static SolidColorBrush SubtleBorder => Brush(IsDark ? "#2E2E36" : "#F0F0EA");
    public static SolidColorBrush NavButtonHover => Brush(IsDark ? "#2E2E38" : "#F0F0EA");

    // Apply theme to a DataGrid
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

    // Apply theme to all TextBlocks inside a panel
    public static void ApplyToPanel(Panel panel, bool isCard = false)
    {
        foreach (var child in panel.Children)
        {
            if (child is TextBlock tb)
            {
                // Keep semantic colors (green/red amounts) — only fix gray/black text
                var current = (tb.Foreground as SolidColorBrush)?.Color;
                if (current == Color.FromRgb(26, 26, 26) ||
                    current == Color.FromRgb(68, 68, 68) ||
                    current == Color.FromRgb(85, 85, 85) ||
                    current == Color.FromRgb(136, 136, 136) ||
                    current == Color.FromRgb(170, 170, 170) ||
                    current == Color.FromRgb(153, 153, 153) ||
                    current == Color.FromRgb(68, 68, 68))
                {
                    tb.Foreground = TextPrimary;
                }
            }
            else if (child is Panel subPanel)
            {
                ApplyToPanel(subPanel, isCard);
            }
        }
    }

    // Apply to ComboBox
    public static void ApplyToComboBox(ComboBox cmb)
    {
        cmb.Background = InputBackground;
        cmb.Foreground = InputForeground;
        cmb.BorderBrush = BorderColor;
    }

    // Apply to TextBox
    public static void ApplyToTextBox(TextBox txt)
    {
        txt.Background = InputBackground;
        txt.Foreground = InputForeground;
        txt.BorderBrush = BorderColor;
        txt.CaretBrush = InputForeground;
    }

    // Apply to Border card
    public static void ApplyToCard(Border card)
    {
        card.Background = CardBackground;
        card.BorderBrush = BorderColor;
    }

    // Walk entire visual tree and theme everything
    public static void ApplyToVisualTree(DependencyObject parent)
    {
        int count = System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < count; i++)
        {
            var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);

            switch (child)
            {
                case TextBlock tb:
                    // Skip semantic colored text (green income, red expense)
                    var color = (tb.Foreground as SolidColorBrush)?.Color;
                    if (color != Color.FromRgb(29, 158, 117) &&
                        color != Color.FromRgb(226, 75, 74) &&
                        color != Color.FromRgb(15, 110, 86) &&
                        color != Color.FromRgb(239, 159, 39) &&
                        color != Color.FromRgb(192, 57, 43) &&
                        color != Color.FromRgb(255, 255, 255))
                    {
                        tb.Foreground = tb.FontSize <= 11
                            ? TextSecondary
                            : TextPrimary;
                    }
                    break;

                case Border border:
                    var bg = (border.Background as SolidColorBrush)?.Color;
                    if (bg == Color.FromRgb(255, 255, 255) ||
                        bg == Color.FromRgb(250, 250, 248) ||
                        bg == Color.FromRgb(240, 240, 234))
                    {
                        border.Background = CardBackground;
                        border.BorderBrush = BorderColor;
                    }
                    break;

                case TextBox txt:
                    txt.Background = InputBackground;
                    txt.Foreground = InputForeground;
                    txt.BorderBrush = BorderColor;
                    txt.CaretBrush = InputForeground;
                    break;

                case ComboBox cmb:
                    cmb.Background = InputBackground;
                    cmb.Foreground = InputForeground;
                    cmb.BorderBrush = BorderColor;
                    break;

                case DataGrid dg:
                    ApplyToDataGrid(dg);
                    break;
            }

            ApplyToVisualTree(child);
        }
    }

    public static void ApplyThemeToWindow(DependencyObject parent)
    {
        ApplyToVisualTree(parent);
    }

    private static SolidColorBrush Brush(string hex) =>
        new((Color)ColorConverter.ConvertFromString(hex));
}