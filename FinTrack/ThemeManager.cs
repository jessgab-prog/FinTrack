using System.Windows;
using System.Windows.Media;

namespace FinTrack;

public static class ThemeManager
{
    public static bool IsDark { get; private set; } = false;

    // Pages subscribe to this to get notified when theme changes
    public static event Action<bool>? ThemeChanged;

    public static void Apply(bool isDark)
    {
        IsDark = isDark;
        ThemeChanged?.Invoke(isDark);
    }

    // Helper colors
    public static SolidColorBrush PageBackground =>
        Brush(IsDark ? "#1A1A1E" : "#F5F5F0");
    public static SolidColorBrush CardBackground =>
        Brush(IsDark ? "#25252B" : "#FFFFFF");
    public static SolidColorBrush TextPrimary =>
        Brush(IsDark ? "#F0F0EC" : "#1A1A1A");
    public static SolidColorBrush TextSecondary =>
        Brush(IsDark ? "#9A9A94" : "#666666");
    public static SolidColorBrush BorderColor =>
        Brush(IsDark ? "#3A3A42" : "#E8E8E2");
    public static SolidColorBrush InputBackground =>
        Brush(IsDark ? "#2E2E36" : "#FFFFFF");
    public static SolidColorBrush AlternateRow =>
        Brush(IsDark ? "#2A2A32" : "#FAFAF8");
    public static SolidColorBrush GridBackground =>
        Brush(IsDark ? "#25252B" : "#FFFFFF");

    private static SolidColorBrush Brush(string hex) =>
        new((Color)ColorConverter.ConvertFromString(hex));
}