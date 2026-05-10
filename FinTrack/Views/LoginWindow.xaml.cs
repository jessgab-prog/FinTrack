using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FinTrack.Data;
using FinTrack.Models;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FinTrack.Views;

public partial class LoginWindow : Window
{
    public static User? LoggedInUser { get; private set; }
    private bool _loginPasswordVisible = false;

    public LoginWindow()
    {
        InitializeComponent();
        TxtUsername.Focus();
    }

    private void BtnToggleLogin_Click(object sender, RoutedEventArgs e)
    {
        _loginPasswordVisible = !_loginPasswordVisible;

        var eyePath = (Path)((Button)sender).Content;

        if (_loginPasswordVisible)
        {
            TxtPasswordVisible.Text = TxtPassword.Password;
            TxtPasswordVisible.Visibility = Visibility.Visible;
            TxtPassword.Visibility = Visibility.Collapsed;

            // Eye with slash — password visible
            eyePath.Data = Geometry.Parse(
                "M12,4.5C7,4.5 2.73,7.61 1,12c1.73,4.39 6,7.5 11,7.5s9.27-3.11 11-7.5C21.27,7.61 17,4.5 12,4.5z " +
                "M12,17c-2.76,0-5-2.24-5-5s2.24-5 5-5 5,2.24 5,5-2.24,5-5,5z M12,9c-1.66,0-3,1.34-3,3s1.34,3 3,3 " +
                "3-1.34 3-3-1.34-3-3-3z " +
                "M2,4.27L4.28,6.55C2.93,7.57 1.82,8.86 1,12c1.73,4.39 6,7.5 11,7.5 " +
                "1.55,0 3.03-0.3 4.38-0.84L17.73,20.5 19.5,18.73 3.77,3 2,4.27z");
            eyePath.Fill = new SolidColorBrush(Color.FromRgb(136, 136, 136));
        }
        else
        {
            TxtPassword.Password = TxtPasswordVisible.Text;
            TxtPassword.Visibility = Visibility.Visible;
            TxtPasswordVisible.Visibility = Visibility.Collapsed;

            // Normal eye — password hidden
            eyePath.Data = Geometry.Parse(
                "M12,4.5C7,4.5 2.73,7.61 1,12c1.73,4.39 6,7.5 11,7.5s9.27-3.11 11-7.5C21.27,7.61 17,4.5 12,4.5z " +
                "M12,17c-2.76,0-5-2.24-5-5s2.24-5 5-5 5,2.24 5,5-2.24,5-5,5z M12,9c-1.66,0-3,1.34-3,3s1.34,3 3,3 " +
                "3-1.34 3-3-1.34-3-3-3z");
            eyePath.Fill = new SolidColorBrush(Color.FromRgb(136, 136, 136));
        }
    }

    private void BtnLogin_Click(object sender, RoutedEventArgs e)
    {
        string username = TxtUsername.Text.Trim();
        string password = _loginPasswordVisible
            ? TxtPasswordVisible.Text
            : TxtPassword.Password;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowError("Please enter both username and password.");
            return;
        }

        using var db = DatabaseHelper.GetContext();
        var user = db.Users.FirstOrDefault(u => u.Username == username);

        if (user == null)
        {
            ShowError("Invalid username or password.");
            return;
        }

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            ShowError("Invalid username or password.");
            return;
        }

        LoggedInUser = new User
        {
            Id = user.Id,
            FullName = user.FullName,
            Username = user.Username,
            PasswordHash = user.PasswordHash,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };

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