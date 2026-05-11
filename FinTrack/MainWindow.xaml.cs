using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FinTrack.Views;

namespace FinTrack;

public partial class MainWindow : Window
{
    private Button? _activeNavButton;
    private bool _isDark = false;

    // Cache pages so they don't reload on theme toggle
    private DashboardPage? _dashboardPage;
    private TransactionsPage? _transactionsPage;
    private ClientsPage? _clientsPage;
    private ReportsPage? _reportsPage;
    private NotificationsPage? _notificationsPage;
    private SettingsPage? _settingsPage;
    private BudgetPage? _budgetPage;

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

        // Use cached pages — only create once
        MainFrame.Navigate(page switch
        {
            "Dashboard" => _dashboardPage ??= new DashboardPage(),
            "Transactions" => _transactionsPage ??= new TransactionsPage(),
            "Clients" => _clientsPage ??= new ClientsPage(),
            "Reports" => _reportsPage ??= new ReportsPage(),
            "Notifications" => _notificationsPage ??= new NotificationsPage(),
            "Settings" => _settingsPage ??= new SettingsPage(),
            "Budget" => _budgetPage ??= new BudgetPage(),
            _ => _dashboardPage ??= new DashboardPage()
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

        // Shell
        this.Background = ThemeManager.PageBackground;
        MainFrame.Background = ThemeManager.PageBackground;
        SidebarBorder.Background = ThemeManager.CardBackground;
        SidebarBorder.BorderBrush = ThemeManager.BorderColor;
        TopBarBorder.Background = ThemeManager.CardBackground;
        TopBarBorder.BorderBrush = ThemeManager.BorderColor;
        TxtPageTitle.Foreground = ThemeManager.TextPrimary;
        TxtUserName.Foreground = ThemeManager.TextPrimary;
        TxtUserRole.Foreground = ThemeManager.TextSecondary;
        TxtLogoName.Foreground = ThemeManager.TextPrimary;
        TxtLogoSub.Foreground = ThemeManager.TextSecondary;

        // Nav section labels
        var sectionColor = ThemeManager.IsDark
            ? new SolidColorBrush(Color.FromRgb(90, 90, 100))
            : new SolidColorBrush(Color.FromRgb(170, 170, 170));
        LblMain.Foreground = sectionColor;
        LblInsights.Foreground = sectionColor;
        LblSettings.Foreground = sectionColor;

        // Nav button text
        var navColor = ThemeManager.IsDark
            ? new SolidColorBrush(Color.FromRgb(180, 180, 190))
            : new SolidColorBrush(Color.FromRgb(68, 68, 68));
        foreach (var btn in new[] { BtnDashboard, BtnTransactions, BtnClients,
                                    BtnReports, BtnNotifications, BtnSettings,
                                    BtnBudget })
        {
            if (btn != _activeNavButton)
                btn.Foreground = navColor;
        }

        // Logout button
        BtnLogout.Background = ThemeManager.IsDark
            ? new SolidColorBrush(Color.FromRgb(80, 30, 30))
            : new SolidColorBrush(Color.FromRgb(253, 236, 234));
        BtnLogout.Foreground = ThemeManager.IsDark
            ? new SolidColorBrush(Color.FromRgb(255, 120, 100))
            : new SolidColorBrush(Color.FromRgb(192, 57, 43));

        // Apply theme to all cached pages that exist
        _dashboardPage?.ApplyTheme();
        _transactionsPage?.ApplyTheme();
        _clientsPage?.ApplyTheme();
        _reportsPage?.ApplyTheme();
        _notificationsPage?.ApplyTheme();
        _settingsPage?.ApplyTheme();
        _budgetPage?.ApplyTheme();
    }

    private void BtnLogout_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            "Are you sure you want to log out?",
            "Log Out",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            var login = new Views.LoginWindow();
            login.Show();
            this.Close();
        }
    }
}