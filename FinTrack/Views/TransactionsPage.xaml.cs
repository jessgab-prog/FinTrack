using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FinTrack.Data;
using FinTrack.Models;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Views;

public partial class TransactionsPage : Page
{
    private List<Transaction> _allTransactions = new();
    private bool _searchFocused = false;

    public TransactionsPage()
    {
        InitializeComponent();
        ApplyTheme();
        LoadTransactions();
    }

    public void ApplyTheme()
    {
        this.Background = ThemeManager.PageBackground;

        if (CardBorder != null)
        {
            CardBorder.Background = ThemeManager.CardBackground;
            CardBorder.BorderBrush = ThemeManager.BorderColor;
        }

        if (TxtSearch != null)
        {
            TxtSearch.Background = ThemeManager.InputBackground;
            TxtSearch.Foreground = ThemeManager.TextSecondary;
            TxtSearch.BorderBrush = ThemeManager.BorderColor;
        }

        if (CmbType != null)
        {
            CmbType.Background = ThemeManager.InputBackground;
            CmbType.Foreground = ThemeManager.InputForeground;
        }

        if (CmbPayment != null)
        {
            CmbPayment.Background = ThemeManager.InputBackground;
            CmbPayment.Foreground = ThemeManager.InputForeground;
        }

        if (TxtSummary != null)
        {
            TxtSummary.Foreground = ThemeManager.TextSecondary;
        }

        if (TxtTransactions != null)
        {
            TxtTransactions.Foreground = ThemeManager.TextPrimary;
        }

        if (DgTransactions != null)
        {
            ThemeManager.ApplyToDataGrid(DgTransactions);
        }

        ThemeManager.ApplyToVisualTree(this);
    }

    private void LoadTransactions()
    {
        using var db = DatabaseHelper.GetContext();
        _allTransactions = db.Transactions
            .Include(t => t.Category)
            .Include(t => t.Client)
            .Where(t => !t.IsDeleted)
            .OrderByDescending(t => t.Date)
            .ToList();
        ApplyFilters();
    }

    private void ApplyFilters()
    {
        if (CmbType == null || CmbPayment == null || TxtSearch == null ||
            DgTransactions == null || TxtSummary == null) return;

        var filtered = _allTransactions.AsEnumerable();

        var search = TxtSearch.Text.Trim();
        if (!string.IsNullOrEmpty(search) && search != "Search transactions...")
            filtered = filtered.Where(t =>
                t.Description.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (t.Category?.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (t.Client?.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false));

        if (CmbType.SelectedItem is ComboBoxItem typeItem &&
            typeItem.Content.ToString() != "All Types")
            filtered = filtered.Where(t => t.Type == typeItem.Content.ToString());

        if (CmbPayment.SelectedItem is ComboBoxItem payItem &&
            payItem.Content.ToString() != "All Methods")
            filtered = filtered.Where(t => t.PaymentMethod == payItem.Content.ToString());

        var result = filtered.ToList();
        DgTransactions.ItemsSource = result;

        var income = result.Where(t => t.Type == "Income").Sum(t => t.Amount);
        var expense = result.Where(t => t.Type == "Expense").Sum(t => t.Amount);
        TxtSummary.Text = $"{result.Count} records  |  " +
                          $"Income: ₱{income:N2}  |  " +
                          $"Expenses: ₱{expense:N2}  |  " +
                          $"Net: ₱{income - expense:N2}";
    }

    private void Filter_Changed(object sender, RoutedEventArgs e) => ApplyFilters();

    private void TxtSearch_GotFocus(object sender, RoutedEventArgs e)
    {
        if (!_searchFocused)
        {
            TxtSearch.Text = "";
            TxtSearch.Foreground = ThemeManager.TextPrimary;
            _searchFocused = true;
        }
    }

    private void TxtSearch_LostFocus(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtSearch.Text))
        {
            TxtSearch.Text = "Search transactions...";
            TxtSearch.Foreground = ThemeManager.TextSecondary;
            _searchFocused = false;
        }
    }

    private void BtnClearFilters_Click(object sender, RoutedEventArgs e)
    {
        TxtSearch.Text = "Search transactions...";
        TxtSearch.Foreground = ThemeManager.TextSecondary;
        _searchFocused = false;
        CmbType.SelectedIndex = 0;
        CmbPayment.SelectedIndex = 0;
        ApplyFilters();
    }

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new TransactionDialog();
        if (dialog.ShowDialog() == true) LoadTransactions();
    }

    private void BtnEdit_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is int id)
        {
            var dialog = new TransactionDialog(id);
            if (dialog.ShowDialog() == true) LoadTransactions();
        }
    }

    private void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is int id)
        {
            var result = MessageBox.Show(
                "Delete this transaction?", "Confirm",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                using var db = DatabaseHelper.GetContext();
                var txn = db.Transactions.Find(id);
                if (txn != null)
                {
                    txn.IsDeleted = true;
                    db.SaveChanges();
                    LoadTransactions();
                }
            }
        }
    }
}