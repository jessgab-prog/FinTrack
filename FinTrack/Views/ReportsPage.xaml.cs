// ReportsPage.xaml.cs
using System.Windows.Controls;
namespace FinTrack.Views;
public partial class ReportsPage : Page
{
    public ReportsPage()
    {
        InitializeComponent();
        this.Background = ThemeManager.PageBackground;
        TxtPlaceholder.Foreground = ThemeManager.TextSecondary;
    }
}