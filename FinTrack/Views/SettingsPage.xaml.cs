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

    public SettingsPage()
    {
        InitializeComponent();
        ApplyTheme();
        LoadProfile();
    }

    public void ApplyTheme()
    {
        this.Background = ThemeManager.PageBackground;
        TxtTitle.Foreground = ThemeManager.TextPrimary;
        CardProfile.Background = ThemeManager.CardBackground;
        CardPassword.Background = ThemeManager.CardBackground;
        CardInfo.Background = ThemeManager.CardBackground;
        CardProfile.BorderBrush = ThemeManager.BorderColor;
        CardPassword.BorderBrush = ThemeManager.BorderColor;
        CardInfo.BorderBrush = ThemeManager.BorderColor;

        // Keep input field values when theme changes
        string fullName = TxtFullName.Text;
        string username = TxtUsername.Text;

        TxtFullName.Background = ThemeManager.InputBackground;
        TxtFullName.Foreground = ThemeManager.TextPrimary;
        TxtUsername.Background = ThemeManager.InputBackground;
        TxtUsername.Foreground = ThemeManager.TextPrimary;

        // Restore values after theme change
        TxtFullName.Text = fullName;
        TxtUsername.Text = username;
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

        string currentPw = _showCurrent
            ? TxtCurrentPwVisible.Text
            : TxtCurrentPw.Password;
        string newPw = _showNew
            ? TxtNewPwVisible.Text
            : TxtNewPw.Password;
        string confirmPw = _showConfirm
            ? TxtConfirmPwVisible.Text
            : TxtConfirmPw.Password;

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

        // Clear all fields
        TxtCurrentPw.Password = "";
        TxtCurrentPwVisible.Text = "";
        TxtNewPw.Password = "";
        TxtNewPwVisible.Text = "";
        TxtConfirmPw.Password = "";
        TxtConfirmPwVisible.Text = "";

        // Reset eye icons back to closed
        _showCurrent = false;
        _showNew = false;
        _showConfirm = false;
        ResetEye(TxtCurrentPw, TxtCurrentPwVisible, EyeCurrent);
        ResetEye(TxtNewPw, TxtNewPwVisible, EyeNew);
        ResetEye(TxtConfirmPw, TxtConfirmPwVisible, EyeConfirm);

        ShowMsg(TxtPwMsg, "Password changed! Use your new password next login.", true);
    }

    // Each button calls its own method with named Path references
    private void BtnToggleCurrent_Click(object sender, RoutedEventArgs e)
    {
        _showCurrent = !_showCurrent;
        ToggleField(_showCurrent,
            TxtCurrentPw, TxtCurrentPwVisible, EyeCurrent);
    }

    private void BtnToggleNew_Click(object sender, RoutedEventArgs e)
    {
        _showNew = !_showNew;
        ToggleField(_showNew,
            TxtNewPw, TxtNewPwVisible, EyeNew);
    }

    private void BtnToggleConfirm_Click(object sender, RoutedEventArgs e)
    {
        _showConfirm = !_showConfirm;
        ToggleField(_showConfirm,
            TxtConfirmPw, TxtConfirmPwVisible, EyeConfirm);
    }

    private static void ToggleField(bool show,
        PasswordBox pwBox, TextBox txBox, Path eyePath)
    {
        if (show)
        {
            txBox.Text = pwBox.Password;
            txBox.Visibility = Visibility.Visible;
            pwBox.Visibility = Visibility.Collapsed;
            eyePath.Data = Geometry.Parse(
                "M2,4.27L4.28,6.55 6.27,8.54C4.5,9.62 3.01,11.17 2,13" +
                "c1.73,3.89 5.5,6.5 10,6.5 1.55,0 3.03-.3 4.38-.84L19,22l1.41-1.41-18-18z" +
                "M12,17c-2.76,0-5-2.24-5-5 0-.77.18-1.5.49-2.14l1.57,1.57C9.03,11.62 9,11.81 9,12" +
                "c0,1.66 1.34,3 3,3 .19,0 .38-.03.56-.07L14.14,16.49C13.5,16.82 12.77,17 12,17z" +
                "M14.97,11.88C14.86,10.29 13.71,9.14 12.12,9.03L14.97,11.88z" +
                "M12,6.5c2.76,0 5,2.24 5,5 0,.51-.1,1-.24,1.46L19.01,15.19" +
                "C19.64,14.04 20,12.77 20,11.5 20,7.61 16.42,4.5 12,4.5" +
                "c-1.04,0-2.03.2-2.94.55L10.8,6.8C11.19,6.62 11.58,6.5 12,6.5z");
        }
        else
        {
            pwBox.Password = txBox.Text;
            pwBox.Visibility = Visibility.Visible;
            txBox.Visibility = Visibility.Collapsed;
            eyePath.Data = Geometry.Parse(
                "M12,6.5c2.76,0 5,2.24 5,5 0,.51-.1,1-.24,1.46l3.06,3.06" +
                "C21.23,14.82 22,13.24 22,11.5 22,7.61 17.42,4.5 12,4.5" +
                "c-1.26,0-2.47.2-3.59.57L10.16,6.82C10.74,6.63 11.35,6.5 12,6.5z" +
                "M2,3.27L4.28,5.55l.46.46C3.08,7.3 2,9.32 2,11.5" +
                "c1.73,3.89 5.5,6.5 10,6.5 1.39,0 2.72-.29 3.93-.82L19,20l1.41-1.41-18-18z" +
                "M12,16.5c-2.76,0-5-2.24-5-5 0-.77.18-1.5.49-2.14l1.57,1.57" +
                "C9.03,11.12 9,11.31 9,11.5c0,1.66 1.34,3 3,3 .19,0 .38-.03.56-.07" +
                "L14.14,15.99C13.5,16.32 12.77,16.5 12,16.5z");
        }
    }

    private static void ResetEye(PasswordBox pwBox, TextBox txBox, Path eyePath)
    {
        pwBox.Visibility = Visibility.Visible;
        txBox.Visibility = Visibility.Collapsed;
        eyePath.Data = Geometry.Parse(
            "M12,6.5c2.76,0 5,2.24 5,5 0,.51-.1,1-.24,1.46l3.06,3.06" +
            "C21.23,14.82 22,13.24 22,11.5 22,7.61 17.42,4.5 12,4.5" +
            "c-1.26,0-2.47.2-3.59.57L10.16,6.82C10.74,6.63 11.35,6.5 12,6.5z" +
            "M2,3.27L4.28,5.55l.46.46C3.08,7.3 2,9.32 2,11.5" +
            "c1.73,3.89 5.5,6.5 10,6.5 1.39,0 2.72-.29 3.93-.82L19,20l1.41-1.41-18-18z" +
            "M12,16.5c-2.76,0-5-2.24-5-5 0-.77.18-1.5.49-2.14l1.57,1.57" +
            "C9.03,11.12 9,11.31 9,11.5c0,1.66 1.34,3 3,3 .19,0 .38-.03.56-.07" +
            "L14.14,15.99C13.5,16.32 12.77,16.5 12,16.5z");
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