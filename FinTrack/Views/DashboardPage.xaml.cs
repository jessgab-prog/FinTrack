using System.Windows.Controls;
using System.Windows.Media;
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

    public void ApplyTheme()
    {
        this.Background = ThemeManager.PageBackground;

        // KPI cards
        foreach (var card in new[] { CardIncome, CardExpense,
                                 CardNet, CardTxn,
                                 CardChart, CardRecent, CardCats })
        {
            if (card != null)
            {
                card.Background = ThemeManager.CardBackground;
                card.BorderBrush = ThemeManager.BorderColor;
            }
        }

        // Primary labels
        foreach (var tb in new[] { TxtWelcome, TxtChartTitle,
                               TxtRecentTitle, TxtCatsTitle,
                               TxtTxnCount, TxtNet })
        {
            if (tb != null)
                tb.Foreground = ThemeManager.TextPrimary;
        }

        // Secondary / muted labels
        foreach (var tb in new[] { TxtDate, TxtIncomeLabel,
                               TxtExpenseLabel, TxtNetLabel2,
                               TxtTxnLabel, TxtTxnSub,
                               TxtIncomeCount, TxtExpenseCount,
                               TxtNetLabel })
        {
            if (tb != null)
                tb.Foreground = ThemeManager.TextSecondary;
        }

        // Apply theme to remaining controls
        ThemeManager.ApplyToVisualTree(this);

        // Chart axis colors
        var labelColor = ThemeManager.IsDark ? "#9A9A94" : "#888888";

        if (BarChart?.XAxes != null)
        {
            foreach (var axis in BarChart.XAxes.OfType<Axis>())
                axis.LabelsPaint = new SolidColorPaint(SKColor.Parse(labelColor));
        }

        if (BarChart?.YAxes != null)
        {
            foreach (var axis in BarChart.YAxes.OfType<Axis>())
                axis.LabelsPaint = new SolidColorPaint(SKColor.Parse(labelColor));
        }
    }

    private void LoadDashboard()
    {
        var user = LoginWindow.LoggedInUser;
        TxtWelcome.Text = $"Welcome back, {user?.FullName ?? "User"} 👋";
        TxtDate.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy");

        using var db = DatabaseHelper.GetContext();

        var allTxns = db.Transactions
            .Include(t => t.Category)
            .Where(t => !t.IsDeleted)
            .ToList();

        var income = allTxns.Where(t => t.Type == "Income").Sum(t => t.Amount);
        var expense = allTxns.Where(t => t.Type == "Expense").Sum(t => t.Amount);
        var net = income - expense;

        TxtIncome.Text = $"₱{income:N2}";
        TxtIncomeCount.Text = $"{allTxns.Count(t => t.Type == "Income")} transactions";
        TxtExpenses.Text = $"₱{expense:N2}";
        TxtExpenseCount.Text = $"{allTxns.Count(t => t.Type == "Expense")} transactions";
        TxtNet.Text = $"₱{net:N2}";
        TxtNet.Foreground = net >= 0
            ? new SolidColorBrush(Color.FromRgb(29, 158, 117))
            : new SolidColorBrush(Color.FromRgb(226, 75, 74));
        TxtNetLabel.Text = net >= 0 ? "Profit this month" : "Loss this month";
        TxtTxnCount.Text = allTxns.Count.ToString();

        RecentList.ItemsSource = allTxns
            .OrderByDescending(t => t.Date)
            .Take(5)
            .ToList();

        LoadChart(allTxns);
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

        var labelColor = ThemeManager.IsDark ? "#9A9A94" : "#888888";
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
                Labels      = labels,
                LabelsPaint = new SolidColorPaint(SKColor.Parse(labelColor)),
                TicksPaint  = new SolidColorPaint(SKColor.Parse(labelColor))
            }
        };

        BarChart.YAxes = new[]
        {
            new Axis
            {
                LabelsPaint = new SolidColorPaint(SKColor.Parse(labelColor)),
                Labeler     = v => $"₱{v:N0}"
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