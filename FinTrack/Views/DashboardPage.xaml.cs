// DashboardPage.xaml.cs
using System.Windows.Controls;
namespace FinTrack.Views;
public partial class DashboardPage : Page
{
    public DashboardPage()
    {
        InitializeComponent();
        this.Background = ThemeManager.PageBackground;
        TxtPlaceholder.Foreground = ThemeManager.TextSecondary;
    }
}