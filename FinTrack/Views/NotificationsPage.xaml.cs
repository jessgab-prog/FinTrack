using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FinTrack.Data;
using FinTrack.Models;

namespace FinTrack.Views;

public partial class NotificationsPage : Page
{
    public NotificationsPage()
    {
        InitializeComponent();
        ApplyTheme();
        ClearOldNotifications();
        GenerateAlerts();
        LoadNotifications();
    }

    private void ApplyTheme()
    {
        this.Background = ThemeManager.PageBackground;
        CardBorder.Background = ThemeManager.CardBackground;
        CardBorder.BorderBrush = ThemeManager.BorderColor;
        TxtTitle.Foreground = ThemeManager.TextPrimary;
        TxtUnread.Foreground = ThemeManager.TextSecondary;
    }

    private void ClearOldNotifications()
    {
        using var db = DatabaseHelper.GetContext();
        var old = db.Notifications.ToList();
        db.Notifications.RemoveRange(old);
        db.SaveChanges();
    }

    private void GenerateAlerts()
    {
        using var db = DatabaseHelper.GetContext();

        var txns = db.Transactions
            .Where(t => !t.IsDeleted)
            .ToList();

        var now = DateTime.Now;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);
        var monthlyIncome = txns.Where(t => t.Type == "Income" && t.Date >= startOfMonth).Sum(t => t.Amount);
        var monthlyExpense = txns.Where(t => t.Type == "Expense" && t.Date >= startOfMonth).Sum(t => t.Amount);
        var totalIncome = txns.Where(t => t.Type == "Income").Sum(t => t.Amount);
        var totalExpense = txns.Where(t => t.Type == "Expense").Sum(t => t.Amount);
        var netProfit = monthlyIncome - monthlyExpense;

        var alerts = new List<Notification>();

        // 1. Great profit month
        if (netProfit > 10000)
            alerts.Add(new Notification
            {
                Message = $"✅ Great month! Your net profit is ₱{netProfit:N2} — keep it up!",
                Type = "Success"
            });

        // 2. Expenses over 80% of income
        if (monthlyIncome > 0 && monthlyExpense >= monthlyIncome * 0.8m)
            alerts.Add(new Notification
            {
                Message = $"⚠️ Warning: Expenses (₱{monthlyExpense:N2}) are over 80% of your income this month.",
                Type = "Warning"
            });

        // 3. Expenses exceed income (loss)
        if (netProfit < 0)
            alerts.Add(new Notification
            {
                Message = $"🔴 Alert: You are running at a loss of ₱{Math.Abs(netProfit):N2} this month. Review your expenses.",
                Type = "Danger"
            });

        // 4. No income this month
        if (monthlyIncome == 0)
            alerts.Add(new Notification
            {
                Message = "ℹ️ No income recorded this month yet. Add your first transaction!",
                Type = "Info"
            });

        // 5. Good overall profit
        if (totalIncome - totalExpense > 50000)
            alerts.Add(new Notification
            {
                Message = $"🏆 Milestone reached! Your total net profit is ₱{totalIncome - totalExpense:N2}.",
                Type = "Success"
            });

        // 6. High single category expense
        var topExpenseCat = txns
            .Where(t => t.Type == "Expense" && t.Date >= startOfMonth)
            .GroupBy(t => t.CategoryId)
            .OrderByDescending(g => g.Sum(t => t.Amount))
            .FirstOrDefault();

        if (topExpenseCat != null)
        {
            var catAmount = topExpenseCat.Sum(t => t.Amount);
            if (catAmount > monthlyIncome * 0.3m && monthlyIncome > 0)
                alerts.Add(new Notification
                {
                    Message = $"📊 One expense category is using over 30% of your monthly income (₱{catAmount:N2}). Consider reviewing.",
                    Type = "Warning"
                });
        }

        // 7. Healthy finances
        if (monthlyIncome > 0 && monthlyExpense < monthlyIncome * 0.5m && netProfit > 0)
            alerts.Add(new Notification
            {
                Message = $"💚 Healthy finances! Your expenses are under 50% of income this month.",
                Type = "Success"
            });

        // 8. GCash heavy usage
        var gcashTotal = txns
            .Where(t => t.PaymentMethod == "GCash" && t.Date >= startOfMonth)
            .Sum(t => t.Amount);
        if (gcashTotal > 5000)
            alerts.Add(new Notification
            {
                Message = $"📱 ₱{gcashTotal:N2} transacted via GCash this month.",
                Type = "Info"
            });

        db.Notifications.AddRange(alerts);
        db.SaveChanges();
    }

    private void LoadNotifications()
    {
        using var db = DatabaseHelper.GetContext();
        var notifs = db.Notifications
            .OrderByDescending(n => n.CreatedAt)
            .ToList();

        NotifList.ItemsSource = notifs;
        var unread = notifs.Count(n => !n.IsRead);
        TxtUnread.Text = unread > 0
            ? $"{unread} unread notification{(unread == 1 ? "" : "s")}"
            : "All caught up!";
    }

    private void BtnMarkAll_Click(object sender, RoutedEventArgs e)
    {
        using var db = DatabaseHelper.GetContext();
        foreach (var n in db.Notifications.Where(n => !n.IsRead))
            n.IsRead = true;
        db.SaveChanges();
        LoadNotifications();
    }
}