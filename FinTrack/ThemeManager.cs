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

    // Known nav/UI colors to skip during tree walk
    private static readonly Color GreenAccent = Color.FromRgb(29, 158, 117);
    private static readonly Color DarkGreen = Color.FromRgb(15, 110, 86);
    private static readonly Color Red = Color.FromRgb(226, 75, 74);
    private static readonly Color DarkRed = Color.FromRgb(192, 57, 43);
    private static readonly Color Amber = Color.FromRgb(239, 159, 39);
    private static readonly Color NavActiveBg = Color.FromRgb(45, 65, 58);
    private static readonly Color NavActiveBgLt = Color.FromRgb(225, 245, 238);
    private static readonly Color LogoutBgDark = Color.FromRgb(80, 30, 30);
    private static readonly Color LogoutBgLight = Color.FromRgb(253, 236, 234);
    private static readonly Color SidebarGreen = Color.FromRgb(29, 158, 117);

    // =========================
    // DATAGRID
    // =========================
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

    // =========================
    // COMBOBOX
    // =========================
    public static void ApplyToComboBox(ComboBox cmb)
    {
        cmb.Background = InputBackground;
        cmb.Foreground = TextPrimary;
        cmb.BorderBrush = BorderColor;

        cmb.Resources[SystemColors.WindowBrushKey] = InputBackground;
        cmb.Resources[SystemColors.ControlBrushKey] = InputBackground;
        cmb.Resources[SystemColors.ControlTextBrushKey] = TextPrimary;
        cmb.Resources[SystemColors.HighlightBrushKey] =
            new SolidColorBrush(GreenAccent);
        cmb.Resources[SystemColors.HighlightTextBrushKey] = Brushes.White;
        cmb.Resources[SystemColors.InactiveSelectionHighlightBrushKey] =
            new SolidColorBrush(GreenAccent);
        cmb.Resources[SystemColors.InactiveSelectionHighlightTextBrushKey] =
            Brushes.White;

        var itemStyle = new Style(typeof(ComboBoxItem));
        itemStyle.Setters.Add(new Setter(Control.BackgroundProperty, InputBackground));
        itemStyle.Setters.Add(new Setter(Control.ForegroundProperty, TextPrimary));
        itemStyle.Setters.Add(new Setter(Control.BorderBrushProperty, BorderColor));

        var hoverTrigger = new Trigger
        { Property = ComboBoxItem.IsMouseOverProperty, Value = true };
        hoverTrigger.Setters.Add(new Setter(Control.BackgroundProperty,
            new SolidColorBrush(Color.FromRgb(36, 118, 87))));
        hoverTrigger.Setters.Add(new Setter(Control.ForegroundProperty, Brushes.White));

        var selectedTrigger = new Trigger
        { Property = ComboBoxItem.IsSelectedProperty, Value = true };
        selectedTrigger.Setters.Add(new Setter(Control.BackgroundProperty,
            new SolidColorBrush(GreenAccent)));
        selectedTrigger.Setters.Add(new Setter(Control.ForegroundProperty, Brushes.White));

        itemStyle.Triggers.Add(hoverTrigger);
        itemStyle.Triggers.Add(selectedTrigger);
        cmb.ItemContainerStyle = itemStyle;
    }

    // =========================
    // TEXTBOX
    // =========================
    public static void ApplyToTextBox(TextBox txt)
    {
        txt.Background = InputBackground;
        txt.Foreground = InputForeground;
        txt.BorderBrush = BorderColor;
        txt.CaretBrush = InputForeground;
    }

    // =========================
    // CARD
    // =========================
    public static void ApplyToCard(Border card)
    {
        card.Background = CardBackground;
        card.BorderBrush = BorderColor;
    }

    // =========================
    // VISUAL TREE — pages only, never the main window shell
    // =========================
    public static void ApplyToVisualTree(DependencyObject parent)
    {
        int count = VisualTreeHelper.GetChildrenCount(parent);

        for (int i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);

            switch (child)
            {
                case TextBlock tb:
                    var color = (tb.Foreground as SolidColorBrush)?.Color;
                    // Skip semantic colors and white (logo, active nav)
                    if (color == GreenAccent || color == DarkGreen ||
                        color == Red || color == DarkRed ||
                        color == Amber || color == Colors.White)
                        break;
                    tb.Foreground = tb.FontSize <= 11
                        ? TextSecondary
                        : TextPrimary;
                    break;

                case Border border:
                    var bg = (border.Background as SolidColorBrush)?.Color;
                    // Skip sidebar green logo border, nav active, logout
                    if (bg == SidebarGreen || bg == NavActiveBg ||
                        bg == NavActiveBgLt || bg == LogoutBgDark ||
                        bg == LogoutBgLight)
                        break;
                    if (bg == Color.FromRgb(255, 255, 255) ||
                        bg == Color.FromRgb(250, 250, 248) ||
                        bg == Color.FromRgb(240, 240, 234))
                    {
                        border.Background = CardBackground;
                        border.BorderBrush = BorderColor;
                    }
                    break;

                case TextBox txt:
                    ApplyToTextBox(txt);
                    break;

                case ComboBox cmb:
                    ApplyToComboBox(cmb);
                    break;

                case Button btn:
                    // NEVER touch nav buttons or logout button
                    var btnBg = (btn.Background as SolidColorBrush)?.Color;
                    if (btnBg == NavActiveBg || btnBg == NavActiveBgLt ||
                        btnBg == LogoutBgDark || btnBg == LogoutBgLight ||
                        btnBg == SidebarGreen)
                        break;
                    // Only theme transparent buttons (page action buttons)
                    if (btn.Background == Brushes.Transparent)
                        btn.Foreground = TextPrimary;
                    break;

                case DataGrid dg:
                    ApplyToDataGrid(dg);
                    break;
            }

            ApplyToVisualTree(child);
        }
    }

    // Only apply to pages — never call this on the MainWindow itself
    public static void ApplyThemeToWindow(DependencyObject parent)
    {
        // Intentionally left empty — use ApplyToVisualTree on pages directly
        // Calling this on MainWindow corrupts sidebar nav button colors
    }

    private static SolidColorBrush Brush(string hex) =>
        new((Color)ColorConverter.ConvertFromString(hex));
}