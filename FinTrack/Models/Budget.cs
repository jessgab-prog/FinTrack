using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinTrack.Models;

public class Budget
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public decimal Amount { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
