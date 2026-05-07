using System.Windows.Controls;
using System.Windows.Media;
using FinTrack.Data;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Views;

public partial class ReportsPage : Page
{
    private bool _loaded = false;

    public ReportsPage()
    {
        InitializeComponent();
        ApplyTheme();
        LoadDropdowns();
        _loaded = true;
        LoadReport();
    }

    private void ApplyTheme()
    {
        this.Background = ThemeManager.PageBackground;
        CardIncome.Background = ThemeManager.CardBackground;
        CardExpense.Background = ThemeManager.CardBackground;
        CardNet.Background = ThemeManager.CardBackground;
        CardTable.Background = ThemeManager.CardBackground;
        CardIncome.BorderBrush = ThemeManager.BorderColor;
        CardExpense.BorderBrush = ThemeManager.BorderColor;
        CardNet.BorderBrush = ThemeManager.BorderColor;
        CardTable.BorderBrush = ThemeManager.BorderColor;
        TxtTitle.Foreground = ThemeManager.TextPrimary;
        TxtTableTitle.Foreground = ThemeManager.TextPrimary;
        DgReport.Background = ThemeManager.CardBackground;
        DgReport.AlternatingRowBackground = ThemeManager.AlternateRow;
        DgReport.Foreground = ThemeManager.TextPrimary;
    }

    private void LoadDropdowns()
    {
        var months = new[]
        {
            "January","February","March","April","May","June",
            "July","August","September","October","November","December"
        };
        CmbMonth.ItemsSource = months;
        CmbMonth.SelectedIndex = DateTime.Now.Month - 1;

        var years = Enumerable.Range(DateTime.Now.Year - 3, 5).ToList();
        CmbYear.ItemsSource = years;
        CmbYear.SelectedItem = DateTime.Now.Year;
    }

    private void LoadReport()
    {
        if (!_loaded) return;

        int month = CmbMonth.SelectedIndex + 1;
        int year = (int)(CmbYear.SelectedItem ?? DateTime.Now.Year);

        using var db = DatabaseHelper.GetContext();
        var txns = db.Transactions
            .Include(t => t.Category)
            .Where(t => !t.IsDeleted &&
                        t.Date.Month == month &&
                        t.Date.Year == year)
            .OrderByDescending(t => t.Date)
            .ToList();

        var income = txns.Where(t => t.Type == "Income").Sum(t => t.Amount);
        var expense = txns.Where(t => t.Type == "Expense").Sum(t => t.Amount);
        var net = income - expense;

        TxtRptIncome.Text = $"₱{income:N2}";
        TxtRptExpense.Text = $"₱{expense:N2}";
        TxtRptNet.Text = $"₱{net:N2}";
        TxtRptNet.Foreground = net >= 0
            ? new SolidColorBrush(Color.FromRgb(29, 158, 117))
            : new SolidColorBrush(Color.FromRgb(226, 75, 74));

        DgReport.ItemsSource = txns;
    }

    private void CmbMonth_Changed(object sender, SelectionChangedEventArgs e) => LoadReport();
    private void CmbYear_Changed(object sender, SelectionChangedEventArgs e) => LoadReport();
}