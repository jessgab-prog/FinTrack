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
        // Reset previous active button
        if (_activeNavButton != null)
        {
            _activeNavButton.Background = Brushes.Transparent;
            _activeNavButton.Foreground = ThemeManager.IsDark
                ? new SolidColorBrush(Color.FromRgb(220, 220, 225))
                : new SolidColorBrush(Color.FromRgb(68, 68, 68));
        }

        // Set new active button
        _activeNavButton = activeBtn;
        activeBtn.Background = ThemeManager.IsDark
            ? new SolidColorBrush(Color.FromRgb(45, 65, 58))
            : new SolidColorBrush(Color.FromRgb(225, 245, 238));
        activeBtn.Foreground =
            new SolidColorBrush(Color.FromRgb(29, 158, 117));

        TxtPageTitle.Text = page;

        // Use cached pages
        Page targetPage = page switch
        {
            "Dashboard" => _dashboardPage ??= new DashboardPage(),
            "Transactions" => _transactionsPage ??= new TransactionsPage(),
            "Clients" => _clientsPage ??= new ClientsPage(),
            "Reports" => _reportsPage ??= new ReportsPage(),
            "Notifications" => _notificationsPage ??= new NotificationsPage(),
            "Settings" => _settingsPage ??= new SettingsPage(),
            "Budget" => _budgetPage ??= new BudgetPage(),
            _ => _dashboardPage ??= new DashboardPage()
        };

        MainFrame.Navigate(targetPage);

        // Always re-apply theme after navigation
        // so new pages get the correct dark/light colors
        Dispatcher.BeginInvoke(new Action(() =>
        {
            _dashboardPage?.ApplyTheme();
            _transactionsPage?.ApplyTheme();
            _clientsPage?.ApplyTheme();
            _reportsPage?.ApplyTheme();
            _notificationsPage?.ApplyTheme();
            _settingsPage?.ApplyTheme();
            _budgetPage?.ApplyTheme();
        }), System.Windows.Threading.DispatcherPriority.Render);
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

        // ── Window / shell ──────────────────────────────
        Background = ThemeManager.PageBackground;
        MainFrame.Background = ThemeManager.PageBackground;
        SidebarBorder.Background = ThemeManager.CardBackground;
        SidebarBorder.BorderBrush = ThemeManager.BorderColor;
        TopBarBorder.Background = ThemeManager.CardBackground;
        TopBarBorder.BorderBrush = ThemeManager.BorderColor;
        TxtPageTitle.Foreground = ThemeManager.TextPrimary;
        TxtUserName.Foreground = ThemeManager.TextPrimary;
        TxtUserRole.Foreground = ThemeManager.TextSecondary;

        // Logo text stays white — it's on green background
        TxtLogoName.Foreground = Brushes.White;
        TxtLogoSub.Foreground = new SolidColorBrush(
            Color.FromRgb(168, 223, 201));

        // User initials
        TxtUserInitials.Foreground =
            new SolidColorBrush(Color.FromRgb(15, 110, 86));

        // ── Section labels ───────────────────────────────
        var sectionColor = ThemeManager.IsDark
            ? new SolidColorBrush(Color.FromRgb(120, 120, 130))
            : new SolidColorBrush(Color.FromRgb(170, 170, 170));
        LblMain.Foreground = sectionColor;
        LblInsights.Foreground = sectionColor;
        LblSettings.Foreground = sectionColor;

        // ── Nav buttons ──────────────────────────────────
        var normalFg = ThemeManager.IsDark
            ? new SolidColorBrush(Color.FromRgb(220, 220, 225))
            : new SolidColorBrush(Color.FromRgb(68, 68, 68));

        var activeBg = ThemeManager.IsDark
            ? new SolidColorBrush(Color.FromRgb(45, 65, 58))
            : new SolidColorBrush(Color.FromRgb(225, 245, 238));

        var activeFg = new SolidColorBrush(Color.FromRgb(29, 158, 117));

        foreach (var btn in new[]
        {
            BtnDashboard, BtnTransactions, BtnClients,
            BtnReports, BtnNotifications, BtnSettings, BtnBudget
        })
        {
            if (btn == _activeNavButton)
            {
                btn.Background = activeBg;
                btn.Foreground = activeFg;
            }
            else
            {
                btn.Background = Brushes.Transparent;
                btn.Foreground = normalFg;
            }
            btn.BorderBrush = Brushes.Transparent;
        }

        // ── Logout button ────────────────────────────────
        BtnLogout.Background = ThemeManager.IsDark
            ? new SolidColorBrush(Color.FromRgb(80, 30, 30))
            : new SolidColorBrush(Color.FromRgb(253, 236, 234));
        BtnLogout.Foreground = ThemeManager.IsDark
            ? new SolidColorBrush(Color.FromRgb(255, 120, 100))
            : new SolidColorBrush(Color.FromRgb(192, 57, 43));

        // ── Top bar buttons ──────────────────────────────
        BtnRefresh.Foreground = ThemeManager.TextPrimary;
        BtnDarkMode.Foreground = ThemeManager.TextPrimary;
        BtnDarkMode.Background = ThemeManager.CardBackground;
        BtnDarkMode.BorderBrush = ThemeManager.BorderColor;

        // ── Apply theme to all cached pages ─────────────
        // This calls each page's own ApplyTheme() which uses
        // ApplyToVisualTree() scoped only to that page —
        // never touching the sidebar or nav buttons
        _dashboardPage?.ApplyTheme();
        _transactionsPage?.ApplyTheme();
        _clientsPage?.ApplyTheme();
        _reportsPage?.ApplyTheme();
        _notificationsPage?.ApplyTheme();
        _settingsPage?.ApplyTheme();
        _budgetPage?.ApplyTheme();
    }

    private void BtnRefresh_Click(object sender, RoutedEventArgs e)
    {
        if (_activeNavButton == null) return;

        string page = _activeNavButton.Tag?.ToString() ?? "Dashboard";

        // Clear cache for current page only
        switch (page)
        {
            case "Dashboard": _dashboardPage = null; break;
            case "Transactions": _transactionsPage = null; break;
            case "Clients": _clientsPage = null; break;
            case "Reports": _reportsPage = null; break;
            case "Notifications": _notificationsPage = null; break;
            case "Settings": _settingsPage = null; break;
            case "Budget": _budgetPage = null; break;
        }

        // Reload the page fresh
        NavigateTo(page, _activeNavButton);

        // Re-apply theme to the newly created page AFTER it loads
        Dispatcher.BeginInvoke(new Action(() =>
        {
            _dashboardPage?.ApplyTheme();
            _transactionsPage?.ApplyTheme();
            _clientsPage?.ApplyTheme();
            _reportsPage?.ApplyTheme();
            _notificationsPage?.ApplyTheme();
            _settingsPage?.ApplyTheme();
            _budgetPage?.ApplyTheme();
        }), System.Windows.Threading.DispatcherPriority.Render);
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