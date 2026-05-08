using System.Windows.Controls;
using System.Windows.Media;      // for SolidColorBrush
using FinTrack.Data;
using FinTrack.Models;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
namespace FinTrack.Views;

public class CategoryStat
{
    public string Name { get; set; } = "";
    public decimal Amount { get; set; }
    public double BarWidth { get; set; }
}

public partial class DashboardPage : Page
{
    public DashboardPage()
    {
        InitializeComponent();
        ApplyTheme();
        LoadDashboard();
    }

    private void ApplyTheme()
    {
        this.Background = ThemeManager.PageBackground;
    }

    private void LoadDashboard()
    {
        var user = Views.LoginWindow.LoggedInUser;
        TxtWelcome.Text = $"Welcome back, {user?.FullName ?? "User"} 👋";
        TxtDate.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy");

        using var db = DatabaseHelper.GetContext();

        var now = DateTime.Now;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);

        var allTxns = db.Transactions
            .Include(t => t.Category)
            .Where(t => !t.IsDeleted)
            .ToList();

        var thisMonth = allTxns;

        // KPI Cards
        var income = thisMonth.Where(t => t.Type == "Income").Sum(t => t.Amount);
        var expense = thisMonth.Where(t => t.Type == "Expense").Sum(t => t.Amount);
        var net = income - expense;

        TxtIncome.Text = $"₱{income:N2}";
        TxtIncomeCount.Text = $"{thisMonth.Count(t => t.Type == "Income")} transactions";
        TxtExpenses.Text = $"₱{expense:N2}";
        TxtExpenseCount.Text = $"{thisMonth.Count(t => t.Type == "Expense")} transactions";
        TxtNet.Text = $"₱{net:N2}";
        TxtNet.Foreground = net >= 0
            ? new SolidColorBrush(System.Windows.Media.Color.FromRgb(29, 158, 117))
            : new SolidColorBrush(System.Windows.Media.Color.FromRgb(226, 75, 74));
        TxtNetLabel.Text = net >= 0 ? "Profit this month" : "Loss this month";
        TxtTxnCount.Text = thisMonth.Count.ToString();

        // Recent transactions (last 5)
        RecentList.ItemsSource = allTxns
            .OrderByDescending(t => t.Date)
            .Take(5)
            .ToList();

        // Chart — last 6 months
        LoadChart(allTxns);

        // Top expense categories
        LoadCategories(allTxns);
    }

    private void LoadChart(List<Transaction> allTxns)
    {
        var months = Enumerable.Range(0, 6)
            .Select(i => DateTime.Now.AddMonths(-5 + i))
            .ToList();

        var incomeVals = months.Select(m =>
            (double)allTxns
                .Where(t => t.Type == "Income" &&
                            t.Date.Year == m.Year &&
                            t.Date.Month == m.Month)
                .Sum(t => t.Amount)).ToArray();

        var expenseVals = months.Select(m =>
            (double)allTxns
                .Where(t => t.Type == "Expense" &&
                            t.Date.Year == m.Year &&
                            t.Date.Month == m.Month)
                .Sum(t => t.Amount)).ToArray();

        var labels = months.Select(m => m.ToString("MMM")).ToArray();

        BarChart.Series = new ISeries[]
        {
            new ColumnSeries<double>
            {
                Name   = "Income",
                Values = incomeVals,
                Fill   = new SolidColorPaint(SKColor.Parse("#1D9E75")),
                Rx = 4, Ry = 4
            },
            new ColumnSeries<double>
            {
                Name   = "Expenses",
                Values = expenseVals,
                Fill   = new SolidColorPaint(SKColor.Parse("#F5C4B3")),
                Rx = 4, Ry = 4
            }
        };

        BarChart.XAxes = new[]
        {
            new Axis
            {
                Labels = labels,
                LabelsPaint = new SolidColorPaint(SKColor.Parse("#888888")),
                TicksPaint  = new SolidColorPaint(SKColor.Parse("#E8E8E2"))
            }
        };

        BarChart.YAxes = new[]
        {
            new Axis
            {
                LabelsPaint = new SolidColorPaint(SKColor.Parse("#888888")),
                Labeler = v => $"₱{v:N0}"
            }
        };
    }

    private void LoadCategories(List<Transaction> allTxns)
    {
        var cats = allTxns
            .Where(t => t.Type == "Expense" && t.Category != null)
            .GroupBy(t => t.Category!.Name)
            .Select(g => new CategoryStat
            {
                Name = g.Key,
                Amount = g.Sum(t => t.Amount)
            })
            .OrderByDescending(c => c.Amount)
            .Take(5)
            .ToList();

        var max = cats.FirstOrDefault()?.Amount ?? 1;
        foreach (var c in cats)
            c.BarWidth = (double)(c.Amount / max) * 300;

        CategoryList.ItemsSource = cats;
    }
}