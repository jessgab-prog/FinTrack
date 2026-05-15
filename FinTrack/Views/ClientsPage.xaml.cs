// ClientsPage.xaml.cs
using System.Windows;
using System.Windows.Controls;
using FinTrack.Data;
using FinTrack.Models;

namespace FinTrack.Views;

public partial class ClientsPage : Page
{
    private List<Client> _all = new();
    private bool _searchFocused = false;

    public ClientsPage()
    {
        InitializeComponent();
        ApplyTheme();
        LoadClients();
    }

    public void ApplyTheme()
    {
        this.Background = ThemeManager.PageBackground;

        if (CardBorder != null)
        {
            CardBorder.Background = ThemeManager.CardBackground;
            CardBorder.BorderBrush = ThemeManager.BorderColor;
        }

        if (DgClients != null)
        {
            DgClients.Background = ThemeManager.GridBackground;
            DgClients.AlternatingRowBackground = ThemeManager.AlternateRow;
            DgClients.Foreground = ThemeManager.TextPrimary;
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

        if (TxtClientsTitle != null)
        {
            TxtClientsTitle.Foreground = ThemeManager.TextPrimary;
        }

        if (DgClients != null)
        {
            ThemeManager.ApplyToDataGrid(DgClients);
        }

        ThemeManager.ApplyToVisualTree(this);
    }
    private void LoadClients()
    {
        using var db = DatabaseHelper.GetContext();
        _all = db.Clients.OrderBy(c => c.Name).ToList();
        ApplyFilters();
    }

    private void ApplyFilters()
    {
        // Prevent null errors during page initialization
        if (TxtSearch == null || CmbType == null || DgClients == null)
            return;

        var filtered = _all.AsEnumerable();

        var search = TxtSearch.Text.Trim();

        if (!string.IsNullOrEmpty(search) &&
            search != "Search clients...")
        {
            filtered = filtered.Where(c =>
                c.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                c.Email.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                c.ContactNumber.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        if (CmbType.SelectedItem is ComboBoxItem item &&
            item.Content.ToString() != "All")
        {
            filtered = filtered.Where(c => c.Type == item.Content.ToString());
        }

        DgClients.ItemsSource = filtered.ToList();
    }
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
            TxtSearch.Text = "Search clients...";
            TxtSearch.Foreground = ThemeManager.TextSecondary;
            _searchFocused = false;
        }
    }

    private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();
    private void CmbType_Changed(object sender, SelectionChangedEventArgs e) => ApplyFilters();

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new ClientDialog();
        if (dialog.ShowDialog() == true) LoadClients();
    }

    private void BtnEdit_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is int id)
        {
            var dialog = new ClientDialog(id);
            if (dialog.ShowDialog() == true) LoadClients();
        }
    }

    private void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is int id)
        {
            using var db = DatabaseHelper.GetContext();

            // Check if this client has linked transactions
            var linkedCount = db.Transactions
                .Count(t => t.ClientId == id && !t.IsDeleted);

            if (linkedCount > 0)
            {
                var confirm = MessageBox.Show(
                    $"This client has {linkedCount} linked transaction(s).\n\n" +
                    $"Deleting will remove the client link from those transactions " +
                    $"but keep the transactions.\n\n" +
                    $"Do you want to continue?",
                    "Linked Transactions Found",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (confirm == MessageBoxResult.No) return;

                // Unlink transactions before deleting client
                var linkedTxns = db.Transactions
                    .Where(t => t.ClientId == id)
                    .ToList();

                foreach (var txn in linkedTxns)
                    txn.ClientId = null;

                db.SaveChanges();
            }
            else
            {
                var confirm = MessageBox.Show(
                    "Delete this client / vendor?",
                    "Confirm",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (confirm == MessageBoxResult.No) return;
            }

            // Now safe to delete
            var client = db.Clients.Find(id);
            if (client != null)
            {
                db.Clients.Remove(client);
                db.SaveChanges();
                LoadClients();
            }
        }
    }
}