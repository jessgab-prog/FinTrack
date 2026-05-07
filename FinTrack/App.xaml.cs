using System.Windows;
using FinTrack.Data;
using Microsoft.EntityFrameworkCore;

namespace FinTrack;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // This auto-creates the DB and applies migrations at runtime
        using (var db = new AppDbContext())
        {
            db.Database.Migrate();
        }

        DatabaseHelper.SeedAdminUser();
    }
}