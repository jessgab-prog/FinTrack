using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinTrack.Models;

public class Transaction
{
    public int Id { get; set; }
    public string Type { get; set; } = "Expense"; // Income or Expense
    public decimal Amount { get; set; }
    public string Description { get; set; } = "";
    public string PaymentMethod { get; set; } = "Cash"; // Cash, GCash, Maya, Bank
    public string ReferenceNumber { get; set; } = "";
    public DateTime Date { get; set; } = DateTime.Now;
    public int? ClientId { get; set; }
    public Client? Client { get; set; }
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
    public bool IsDeleted { get; set; } = false;
}