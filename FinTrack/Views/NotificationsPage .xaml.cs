using System.Windows;
using System.Windows.Controls;
using FinTrack.Data;
using FinTrack.Models;

namespace FinTrack.Views;

public partial class NotificationsPage : Page
{
    public NotificationsPage()
    {
        InitializeComponent();
        ApplyTheme();
        LoadNotifications();
        GenerateAlerts();
    }

    private void ApplyTheme()
    {
        this.Background = ThemeManager.PageBackground;
        CardBorder.Background = ThemeManager.CardBackground;
        CardBorder.BorderBrush = ThemeManager.BorderColor;
        TxtTitle.Foreground = ThemeManager.TextPrimary;
        TxtUnread.Foreground = ThemeManager.TextSecondary;
    }

    private void GenerateAlerts()
    {
        using var db = DatabaseHelper.GetContext();

        // Check if we already generated today's alerts
        var old = db.Notifications.ToList();
        db.Notifications.RemoveRange(old);
        db.SaveChanges();

        var txns = db.Transactions
            .Where(t => !t.IsDeleted)
            .ToList();

        var now = DateTime.Now;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);

        var income = txns.Where(t => t.Type == "Income" && t.Date >= startOfMonth).Sum(t => t.Amount);
        var expense = txns.Where(t => t.Type == "Expense" && t.Date >= startOfMonth).Sum(t => t.Amount);

        // Expense warning
        if (expense > income * 0.8m && income > 0)
            db.Notifications.Add(new Notification
            {
                Message = $"⚠️ Expenses (₱{expense:N2}) are over 80% of income this month.",
                Type = "Warning"
            });

        // Good profit notification
        if (income - expense > 10000)
            db.Notifications.Add(new Notification
            {
                Message = $"✅ Great month! Net profit is ₱{income - expense:N2}.",
                Type = "Success"
            });

        // No income this month
        if (income == 0)
            db.Notifications.Add(new Notification
            {
                Message = "ℹ️ No income recorded this month yet. Add your first transaction!",
                Type = "Info"
            });

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