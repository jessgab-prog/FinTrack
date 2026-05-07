using System.Windows;
using FinTrack.Data;
using FinTrack.Models;

namespace FinTrack.Views;

public partial class TransactionDialog : Window
{
    private readonly int? _editId;

    public TransactionDialog(int? editId = null)
    {
        InitializeComponent();
        _editId = editId;
        LoadDropdowns();

        if (editId.HasValue)
        {
            TxtTitle.Text = "Edit Transaction";
            LoadExisting(editId.Value);
        }
        else
        {
            DpDate.SelectedDate = DateTime.Today;
        }
    }

    private void LoadDropdowns()
    {
        using var db = DatabaseHelper.GetContext();

        CmbCategory.ItemsSource = db.Categories.ToList();
        CmbCategory.SelectedIndex = 0;

        var clients = db.Clients.ToList();
        clients.Insert(0, new Client { Id = 0, Name = "(None)" });
        CmbClient.ItemsSource = clients;
        CmbClient.SelectedIndex = 0;
    }

    private void LoadExisting(int id)
    {
        using var db = DatabaseHelper.GetContext();
        var t = db.Transactions.Find(id);
        if (t == null) return;

        CmbType.SelectedIndex = t.Type == "Income" ? 0 : 1;
        TxtDescription.Text = t.Description;
        TxtAmount.Text = t.Amount.ToString("N2");
        CmbPayment.SelectedIndex = t.PaymentMethod switch
        {
            "GCash" => 1,
            "Maya" => 2,
            "Bank" => 3,
            _ => 0
        };
        CmbCategory.SelectedValue = t.CategoryId;
        CmbClient.SelectedValue = t.ClientId ?? 0;
        DpDate.SelectedDate = t.Date;
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        // Validate
        if (string.IsNullOrWhiteSpace(TxtDescription.Text))
        { ShowError("Description is required."); return; }

        if (!decimal.TryParse(TxtAmount.Text, out var amount) || amount <= 0)
        { ShowError("Enter a valid amount."); return; }

        if (DpDate.SelectedDate == null)
        { ShowError("Please select a date."); return; }

        using var db = DatabaseHelper.GetContext();

        Transaction txn = _editId.HasValue
            ? db.Transactions.Find(_editId.Value)!
            : new Transaction();

        txn.Type = (CmbType.SelectedItem as System.Windows.Controls.ComboBoxItem)!
                             .Content.ToString()!;
        txn.Description = TxtDescription.Text.Trim();
        txn.Amount = amount;
        txn.PaymentMethod = (CmbPayment.SelectedItem as System.Windows.Controls.ComboBoxItem)!
                             .Content.ToString()!;
        txn.CategoryId = (int?)CmbCategory.SelectedValue;
        txn.ClientId = (int?)CmbClient.SelectedValue == 0
                             ? null : (int?)CmbClient.SelectedValue;
        txn.Date = DpDate.SelectedDate!.Value;

        if (!_editId.HasValue) db.Transactions.Add(txn);
        db.SaveChanges();

        DialogResult = true;
        Close();
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void ShowError(string msg)
    {
        TxtError.Text = msg;
        TxtError.Visibility = Visibility.Visible;
    }
}