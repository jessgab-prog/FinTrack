using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using FinTrack.Models;

namespace FinTrack.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Budget> Budgets { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=fintrack.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Prevent cascade delete on Client → Transactions
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Client)
            .WithMany()
            .HasForeignKey(t => t.ClientId)
            .OnDelete(DeleteBehavior.SetNull);

        // Prevent cascade delete on Category → Transactions
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Category)
            .WithMany()
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Sales", Type = "Income" },
            new Category { Id = 2, Name = "Freelance", Type = "Income" },
            new Category { Id = 3, Name = "Other Income", Type = "Income" },
            new Category { Id = 4, Name = "Operations", Type = "Expense" },
            new Category { Id = 5, Name = "Utilities", Type = "Expense" },
            new Category { Id = 6, Name = "Advertising", Type = "Expense" },
            new Category { Id = 7, Name = "Software / Tools", Type = "Expense" },
            new Category { Id = 8, Name = "Office Supplies", Type = "Expense" }
        );
    }
}
