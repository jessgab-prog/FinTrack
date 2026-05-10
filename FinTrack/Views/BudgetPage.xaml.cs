using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FinTrack.Data;
using FinTrack.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Views;

public partial class BudgetPage : Page
{
    private bool _loaded = false;

    public BudgetPage()
    {
        InitializeComponent();
        ApplyTheme();
        LoadDropdowns();
        _loaded = true;
        LoadBudgets();
    }

    private void ApplyTheme()
    {
        this.Background = ThemeManager.PageBackground;
        TxtTitle.Foreground = ThemeManager.TextPrimary;
        TxtSubtitle.Foreground = ThemeManager.TextSecondary;
        CardTotalBudget.Background = ThemeManager.CardBackground;
        CardTotalSpent.Background = ThemeManager.CardBackground;
        CardRemaining.Background = ThemeManager.CardBackground;
        CardBudgets.Background = ThemeManager.CardBackground;
        CardTotalBudget.BorderBrush = ThemeManager.BorderColor;
        CardTotalSpent.BorderBrush = ThemeManager.BorderColor;
        CardRemaining.BorderBrush = ThemeManager.BorderColor;
        CardBudgets.BorderBrush = ThemeManager.BorderColor;
    }

    private void LoadDropdowns()
    {
        CmbMonth.ItemsSource = new[]
        {
            "January","February","March","April","May","June",
            "July","August","September","October","November","December"
        };
        CmbMonth.SelectedIndex = DateTime.Now.Month - 1;

        CmbYear.ItemsSource = Enumerable.Range(DateTime.Now.Year - 1, 4).ToList();
        CmbYear.SelectedItem = DateTime.Now.Year;
    }

    private void LoadBudgets()
    {
        if (!_loaded) return;

        int month = CmbMonth.SelectedIndex + 1;
        int year = (int)(CmbYear.SelectedItem ?? DateTime.Now.Year);

        using var db = DatabaseHelper.GetContext();

        var budgets = db.Budgets
            .Include(b => b.Category)
            .Where(b => b.Month == month && b.Year == year)
            .ToList();

        var transactions = db.Transactions
            .Where(t => !t.IsDeleted &&
                        t.Type == "Expense" &&
                        t.Date.Month == month &&
                        t.Date.Year == year)
            .ToList();

        var viewModels = budgets.Select(b =>
        {
            var spent = transactions
                .Where(t => t.CategoryId == b.CategoryId)
                .Sum(t => t.Amount);

            return new BudgetViewModel
            {
                Id = b.Id,
                CategoryId = b.CategoryId,
                CategoryName = b.Category?.Name ?? "Unknown",
                BudgetAmount = b.Amount,
                SpentAmount = spent
            };
        }).OrderByDescending(b => b.ProgressPercent).ToList();

        BudgetList.ItemsSource = viewModels;

        // Show/hide empty state
        EmptyState.Visibility = viewModels.Count == 0
            ? Visibility.Visible
            : Visibility.Collapsed;

        // Summary cards
        var totalBudget = viewModels.Sum(b => b.BudgetAmount);
        var totalSpent = viewModels.Sum(b => b.SpentAmount);
        var remaining = totalBudget - totalSpent;

        TxtTotalBudget.Text = $"₱{totalBudget:N2}";
        TxtTotalSpent.Text = $"₱{totalSpent:N2}";
        TxtRemaining.Text = $"₱{Math.Abs(remaining):N2}";
        TxtRemaining.Foreground = remaining >= 0
            ? new SolidColorBrush(Color.FromRgb(29, 158, 117))
            : new SolidColorBrush(Color.FromRgb(226, 75, 74));
        TxtRemainingLabel.Text = remaining >= 0
            ? "Budget left to spend"
            : "Over budget!";
    }

    private void CmbMonth_Changed(object sender, SelectionChangedEventArgs e) => LoadBudgets();
    private void CmbYear_Changed(object sender, SelectionChangedEventArgs e) => LoadBudgets();

    private void BtnAddBudget_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new BudgetDialog();
        if (dialog.ShowDialog() == true) LoadBudgets();
    }

    private void BtnEditBudget_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is int id)
        {
            var dialog = new BudgetDialog(id);
            if (dialog.ShowDialog() == true) LoadBudgets();
        }
    }

    private void BtnDeleteBudget_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is int id)
        {
            var result = MessageBox.Show(
                "Delete this budget?", "Confirm",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                using var db = DatabaseHelper.GetContext();
                var budget = db.Budgets.Find(id);
                if (budget != null)
                {
                    db.Budgets.Remove(budget);
                    db.SaveChanges();
                    LoadBudgets();
                }
            }
        }
    }
}