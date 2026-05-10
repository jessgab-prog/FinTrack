using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinTrack.Models;

namespace FinTrack.Data;

public static class DatabaseHelper
{
    public static AppDbContext GetContext() => new AppDbContext();

    public static void SeedAdminUser()
    {
        using var db = GetContext();
        if (!db.Users.Any())
        {
            db.Users.Add(new User
            {
                FullName = "Admin",
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = "Owner"
            });
            db.SaveChanges();
        }
    }
}