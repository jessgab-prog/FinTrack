using System.Windows.Media;

namespace FinTrack.ViewModels;

public class BudgetViewModel
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = "";
    public decimal BudgetAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public decimal RemainingAmount => BudgetAmount - SpentAmount;
    public double ProgressPercent => BudgetAmount > 0
        ? Math.Min((double)(SpentAmount / BudgetAmount) * 100, 100)
        : 0;
    public double ProgressWidth => BudgetAmount > 0
        ? Math.Min((double)(SpentAmount / BudgetAmount) * 400, 400)
        : 0;

    public SolidColorBrush ProgressColor => ProgressPercent switch
    {
        >= 100 => new SolidColorBrush(Color.FromRgb(226, 75, 74)),
        >= 80 => new SolidColorBrush(Color.FromRgb(239, 159, 39)),
        _ => new SolidColorBrush(Color.FromRgb(29, 158, 117))
    };

    public SolidColorBrush SpentColor => ProgressPercent >= 100
        ? new SolidColorBrush(Color.FromRgb(226, 75, 74))
        : new SolidColorBrush(Color.FromRgb(68, 68, 68));

    public SolidColorBrush RemainingColor => RemainingAmount < 0
        ? new SolidColorBrush(Color.FromRgb(226, 75, 74))
        : new SolidColorBrush(Color.FromRgb(29, 158, 117));

    public string ProgressText => $"{ProgressPercent:N0}% used";

    public string StatusText => ProgressPercent switch
    {
        >= 100 => "⛔ Over budget",
        >= 80 => "⚠️ Almost at limit",
        >= 50 => "🟡 Halfway through",
        _ => "✅ On track"
    };

    public SolidColorBrush StatusColor => ProgressPercent switch
    {
        >= 100 => new SolidColorBrush(Color.FromRgb(226, 75, 74)),
        >= 80 => new SolidColorBrush(Color.FromRgb(239, 159, 39)),
        >= 50 => new SolidColorBrush(Color.FromRgb(186, 117, 23)),
        _ => new SolidColorBrush(Color.FromRgb(29, 158, 117))
    };
}