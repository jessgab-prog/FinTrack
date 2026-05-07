using System.Windows;
using FinTrack.Data;
using FinTrack.Models;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Views;

public partial class LoginWindow : Window
{
    public static User? LoggedInUser { get; private set; }

    public LoginWindow()
    {
        InitializeComponent();
        TxtUsername.Focus();
    }

    private void BtnLogin_Click(object sender, RoutedEventArgs e)
    {
        string username = TxtUsername.Text.Trim();
        string password = TxtPassword.Password;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowError("Please enter both username and password.");
            return;
        }

        using var db = DatabaseHelper.GetContext();
        var user = db.Users.FirstOrDefault(u => u.Username == username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            ShowError("Invalid username or password.");
            return;
        }

        LoggedInUser = user;

        var main = new MainWindow();
        main.Show();
        this.Close();
    }

    private void ShowError(string message)
    {
        TxtError.Text = message;
        TxtError.Visibility = Visibility.Visible;
    }
}