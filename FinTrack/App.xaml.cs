using System.Windows;
using FinTrack.Data;

namespace FinTrack;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        using (var db = new AppDbContext())
        {
            db.Database.EnsureCreated();
        }

        DatabaseHelper.SeedAdminUser();
    }
}