// NotificationsPage.xaml.cs
using System.Windows.Controls;
namespace FinTrack.Views;
public partial class NotificationsPage : Page
{
    public NotificationsPage()
    {
        InitializeComponent();
        this.Background = ThemeManager.PageBackground;
        TxtPlaceholder.Foreground = ThemeManager.TextSecondary;
    }
}