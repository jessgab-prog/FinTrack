using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FinTrack.Data;
using FinTrack.Views;

namespace FinTrack.Views;

public partial class SettingsPage : Page
{
    private bool _showCurrent = false;
    private bool _showNew = false;
    private bool _showConfirm = false;

    // Eye open path data
    private const string EyeOpen =
        "M12,4.5C7,4.5 2.73,7.61 1,12c1.73,4.39 6,7.5 11,7.5s9.27-3.11 11-7.5" +
        "C21.27,7.61 17,4.5 12,4.5z M12,17c-2.76,0-5-2.24-5-5s2.24-5 5-5 5,2.24 5,5-2.24,5-5,5z" +
        "M12,9c-1.66,0-3,1.34-3,3s1.34,3 3,3 3-1.34 3-3-1.34-3-3-3z";

    // Eye closed/slash path data
    private const string EyeSlash =
        "M12,4.5C7,4.5 2.73,7.61 1,12c1.73,4.39 6,7.5 11,7.5s9.27-3.11 11-7.5" +
        "C21.27,7.61 17,4.5 12,4.5z M12,17c-2.76,0-5-2.24-5-5s2.24-5 5-5 5,2.24 5,5-2.24,5-5,5z" +
        "M12,9c-1.66,0-3,1.34-3,3s1.34,3 3,3 3-1.34 3-3-1.34-3-3-3z" +
        "M2,4.27L4.28,6.55C2.93,7.57 1.82,8.86 1,12c1.73,4.39 6,7.5 11,7.5" +
        "1.55,0 3.03-0.3 4.38-0.84L17.73,20.5 19.5,18.73 3.77,3 2,4.27z";

    public SettingsPage()
    {
        InitializeComponent();
        ApplyTheme();
        LoadProfile();
    }

    private void ApplyTheme()
    {
        this.Background = ThemeManager.PageBackground;
        TxtTitle.Foreground = ThemeManager.TextPrimary;
        CardProfile.Background = ThemeManager.CardBackground;
        CardPassword.Background = ThemeManager.CardBackground;
        CardInfo.Background = ThemeManager.CardBackground;
        CardProfile.BorderBrush = ThemeManager.BorderColor;
        CardPassword.BorderBrush = ThemeManager.BorderColor;
        CardInfo.BorderBrush = ThemeManager.BorderColor;
        TxtFullName.Background = ThemeManager.InputBackground;
        TxtFullName.Foreground = ThemeManager.TextPrimary;
        TxtUsername.Background = ThemeManager.InputBackground;
        TxtUsername.Foreground = ThemeManager.TextPrimary;
    }

    private void LoadProfile()
    {
        var user = LoginWindow.LoggedInUser;
        if (user == null) return;

        using var db = DatabaseHelper.GetContext();
        var dbUser = db.Users.Find(user.Id);
        if (dbUser == null) return;

        TxtFullName.Text = dbUser.FullName;
        TxtUsername.Text = dbUser.Username;
    }

    private void BtnSaveProfile_Click(object sender, RoutedEventArgs e)
    {
        var sessionUser = LoginWindow.LoggedInUser;
        if (sessionUser == null) return;

        string newFullName = TxtFullName.Text.Trim();
        string newUsername = TxtUsername.Text.Trim();

        if (string.IsNullOrWhiteSpace(newFullName))
        { ShowMsg(TxtProfileMsg, "Full name cannot be empty.", false); return; }

        if (string.IsNullOrWhiteSpace(newUsername))
        { ShowMsg(TxtProfileMsg, "Username cannot be empty.", false); return; }

        using var db = DatabaseHelper.GetContext();

        var existing = db.Users.FirstOrDefault(u =>
            u.Username == newUsername && u.Id != sessionUser.Id);
        if (existing != null)
        { ShowMsg(TxtProfileMsg, "That username is already taken.", false); return; }

        var dbUser = db.Users.Find(sessionUser.Id);
        if (dbUser == null) return;

        dbUser.FullName = newFullName;
        dbUser.Username = newUsername;
        db.SaveChanges();

        sessionUser.FullName = newFullName;
        sessionUser.Username = newUsername;

        ShowMsg(TxtProfileMsg, "Profile saved successfully!", true);
    }

    private void BtnChangePw_Click(object sender, RoutedEventArgs e)
    {
        var sessionUser = LoginWindow.LoggedInUser;
        if (sessionUser == null) return;

        string currentPw = _showCurrent ? TxtCurrentPwVisible.Text : TxtCurrentPw.Password;
        string newPw = _showNew ? TxtNewPwVisible.Text : TxtNewPw.Password;
        string confirmPw = _showConfirm ? TxtConfirmPwVisible.Text : TxtConfirmPw.Password;

        if (string.IsNullOrWhiteSpace(currentPw))
        { ShowMsg(TxtPwMsg, "Please enter your current password.", false); return; }

        if (string.IsNullOrWhiteSpace(newPw))
        { ShowMsg(TxtPwMsg, "Please enter a new password.", false); return; }

        using var db = DatabaseHelper.GetContext();
        var dbUser = db.Users.Find(sessionUser.Id);
        if (dbUser == null) return;

        if (!BCrypt.Net.BCrypt.Verify(currentPw, dbUser.PasswordHash))
        { ShowMsg(TxtPwMsg, "Current password is incorrect.", false); return; }

        if (newPw.Length < 6)
        { ShowMsg(TxtPwMsg, "New password must be at least 6 characters.", false); return; }

        if (newPw != confirmPw)
        { ShowMsg(TxtPwMsg, "New passwords do not match.", false); return; }

        string newHash = BCrypt.Net.BCrypt.HashPassword(newPw);
        dbUser.PasswordHash = newHash;
        db.SaveChanges();
        sessionUser.PasswordHash = newHash;

        TxtCurrentPw.Password = TxtCurrentPwVisible.Text = "";
        TxtNewPw.Password = TxtNewPwVisible.Text = "";
        TxtConfirmPw.Password = TxtConfirmPwVisible.Text = "";

        ShowMsg(TxtPwMsg, "Password changed! Use your new password next login.", true);
    }

    // Toggle handlers — each calls the shared helper
    private void BtnToggleCurrent_Click(object sender, RoutedEventArgs e)
        => Toggle(sender, ref _showCurrent, TxtCurrentPw, TxtCurrentPwVisible);

    private void BtnToggleNew_Click(object sender, RoutedEventArgs e)
        => Toggle(sender, ref _showNew, TxtNewPw, TxtNewPwVisible);

    private void BtnToggleConfirm_Click(object sender, RoutedEventArgs e)
        => Toggle(sender, ref _showConfirm, TxtConfirmPw, TxtConfirmPwVisible);

    private static void Toggle(object sender, ref bool isVisible,
        PasswordBox pwBox, TextBox txBox)
    {
        isVisible = !isVisible;

        // Get the Path icon inside the button
        var btn = (Button)sender;
        var path = btn.Content as Path;

        if (isVisible)
        {
            txBox.Text = pwBox.Password;
            txBox.Visibility = Visibility.Visible;
            pwBox.Visibility = Visibility.Collapsed;
            if (path != null)
                path.Data = Geometry.Parse(EyeSlash);
        }
        else
        {
            pwBox.Password = txBox.Text;
            pwBox.Visibility = Visibility.Visible;
            txBox.Visibility = Visibility.Collapsed;
            if (path != null)
                path.Data = Geometry.Parse(EyeOpen);
        }
    }

    private static void ShowMsg(TextBlock tb, string msg, bool success)
    {
        tb.Text = msg;
        tb.Foreground = success
            ? new SolidColorBrush(Color.FromRgb(29, 158, 117))
            : new SolidColorBrush(Color.FromRgb(226, 75, 74));
        tb.Visibility = Visibility.Visible;
    }
}