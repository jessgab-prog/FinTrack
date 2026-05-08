using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FinTrack.Data;
using FinTrack.Views;

namespace FinTrack.Views;

public partial class SettingsPage : Page
{
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
        TxtFullName.Text = user.FullName;
        TxtUsername.Text = user.Username;
    }

    private void BtnSaveProfile_Click(object sender, RoutedEventArgs e)
    {
        var user = LoginWindow.LoggedInUser;
        if (user == null) return;

        if (string.IsNullOrWhiteSpace(TxtFullName.Text) ||
            string.IsNullOrWhiteSpace(TxtUsername.Text))
        {
            ShowMsg(TxtProfileMsg, "All fields are required.", false);
            return;
        }

        using var db = DatabaseHelper.GetContext();
        var dbUser = db.Users.Find(user.Id);
        if (dbUser == null) return;

        dbUser.FullName = TxtFullName.Text.Trim();
        dbUser.Username = TxtUsername.Text.Trim();
        db.SaveChanges();

        // Update session
        user.FullName = dbUser.FullName;
        user.Username = dbUser.Username;

        ShowMsg(TxtProfileMsg, "Profile updated successfully!", true);
    }

    private void BtnChangePw_Click(object sender, RoutedEventArgs e)
    {
        var user = LoginWindow.LoggedInUser;
        if (user == null) return;

        if (!BCrypt.Net.BCrypt.Verify(TxtCurrentPw.Password, user.PasswordHash))
        {
            ShowMsg(TxtPwMsg, "Current password is incorrect.", false);
            return;
        }

        if (TxtNewPw.Password.Length < 6)
        {
            ShowMsg(TxtPwMsg, "New password must be at least 6 characters.", false);
            return;
        }

        if (TxtNewPw.Password != TxtConfirmPw.Password)
        {
            ShowMsg(TxtPwMsg, "Passwords do not match.", false);
            return;
        }

        using var db = DatabaseHelper.GetContext();
        var dbUser = db.Users.Find(user.Id);
        if (dbUser == null) return;

        dbUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(TxtNewPw.Password);
        db.SaveChanges();
        user.PasswordHash = dbUser.PasswordHash;

        TxtCurrentPw.Password = "";
        TxtNewPw.Password = "";
        TxtConfirmPw.Password = "";

        ShowMsg(TxtPwMsg, "Password changed successfully!", true);
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