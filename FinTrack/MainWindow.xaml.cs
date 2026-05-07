using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using FinTrack.Views;

namespace FinTrack;

public partial class MainWindow : Window
{
    private Button? _activeNavButton;
    private bool _isDark = false;

    public MainWindow()
    {
        InitializeComponent();
        LoadUserInfo();
        NavigateTo("Dashboard", BtnDashboard);
    }

    private void LoadUserInfo()
    {
        var user = LoginWindow.LoggedInUser;
        if (user == null) return;
        TxtUserName.Text = user.FullName;
        TxtUserRole.Text = user.Role;
        var parts = user.FullName.Split(' ');
        TxtUserInitials.Text = parts.Length >= 2
            ? $"{parts[0][0]}{parts[1][0]}"
            : user.FullName[..1].ToUpper();
    }

    private void NavButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn)
            NavigateTo(btn.Tag?.ToString() ?? "Dashboard", btn);
    }

    private void NavigateTo(string page, Button activeBtn)
    {
        if (_activeNavButton != null)
        {
            _activeNavButton.Background = Brushes.Transparent;
            _activeNavButton.Foreground = new SolidColorBrush(Color.FromRgb(68, 68, 68));
        }

        _activeNavButton = activeBtn;
        activeBtn.Background = new SolidColorBrush(Color.FromRgb(225, 245, 238));
        activeBtn.Foreground = new SolidColorBrush(Color.FromRgb(15, 110, 86));
        TxtPageTitle.Text = page;

        MainFrame.Navigate(page switch
        {
            "Dashboard" => new DashboardPage(),
            "Transactions" => new TransactionsPage(),
            "Clients" => new ClientsPage(),
            "Reports" => new ReportsPage(),
            "Notifications" => new NotificationsPage(),
            _ => new DashboardPage()
        });
    }

    private void BtnDarkMode_Checked(object sender, RoutedEventArgs e)
    {
        _isDark = true;
        BtnDarkMode.Content = "☀️  Light";
        ApplyTheme();
    }

    private void BtnDarkMode_Unchecked(object sender, RoutedEventArgs e)
    {
        _isDark = false;
        BtnDarkMode.Content = "🌙  Dark";
        ApplyTheme();
    }

    private void ApplyTheme()
    {
        ThemeManager.Apply(_isDark);

        // Shell colors
        this.Background = ThemeManager.PageBackground;
        MainFrame.Background = ThemeManager.PageBackground;
        SidebarBorder.Background = ThemeManager.CardBackground;
        SidebarBorder.BorderBrush = ThemeManager.BorderColor;
        TopBarBorder.Background = ThemeManager.CardBackground;
        TopBarBorder.BorderBrush = ThemeManager.BorderColor;
        TxtPageTitle.Foreground = ThemeManager.TextPrimary;
        TxtUserName.Foreground = ThemeManager.TextPrimary;
        TxtUserRole.Foreground = ThemeManager.TextSecondary;

        // Re-navigate to current page so it reloads with new theme
        if (_activeNavButton != null)
            NavButton_Click(_activeNavButton, new RoutedEventArgs());
    }

    private static SolidColorBrush Brush(string hex) =>
        new((Color)ColorConverter.ConvertFromString(hex));
}