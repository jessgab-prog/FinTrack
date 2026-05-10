using System.Windows;
using FinTrack.Data;
using FinTrack.Models;

namespace FinTrack.Views;

public partial class BudgetDialog : Window
{
    private readonly int? _editId;

    public BudgetDialog(int? editId = null)
    {
        InitializeComponent();
        _editId = editId;
        LoadDropdowns();
        if (editId.HasValue) LoadExisting(editId.Value);
    }

    private void LoadDropdowns()
    {
        using var db = DatabaseHelper.GetContext();
        CmbCategory.ItemsSource = db.Categories
            .Where(c => c.Type == "Expense")
            .ToList();
        CmbCategory.SelectedIndex = 0;

        CmbMonth.ItemsSource = new[]
        {
            "January","February","March","April","May","June",
            "July","August","September","October","November","December"
        };
        CmbMonth.SelectedIndex = DateTime.Now.Month - 1;

        CmbYear.ItemsSource = Enumerable.Range(DateTime.Now.Year, 3).ToList();
        CmbYear.SelectedIndex = 0;
    }

    private void LoadExisting(int id)
    {
        TxtTitle.Text = "Edit Budget";
        using var db = DatabaseHelper.GetContext();
        var b = db.Budgets.Find(id);
        if (b == null) return;
        CmbCategory.SelectedValue = b.CategoryId;
        TxtAmount.Text = b.Amount.ToString("N2");
        CmbMonth.SelectedIndex = b.Month - 1;
        CmbYear.SelectedValue = b.Year;
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        if (!decimal.TryParse(TxtAmount.Text, out var amount) || amount <= 0)
        { ShowError("Enter a valid budget amount."); return; }

        if (CmbCategory.SelectedValue == null)
        { ShowError("Please select a category."); return; }

        using var db = DatabaseHelper.GetContext();

        Budget budget = _editId.HasValue
            ? db.Budgets.Find(_editId.Value)!
            : new Budget();

        budget.CategoryId = (int)CmbCategory.SelectedValue;
        budget.Amount = amount;
        budget.Month = CmbMonth.SelectedIndex + 1;
        budget.Year = (int)CmbYear.SelectedItem;

        if (!_editId.HasValue) db.Budgets.Add(budget);
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