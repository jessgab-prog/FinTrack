using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinTrack.Models;

public class Client
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string ContactNumber { get; set; } = "";
    public string Email { get; set; } = "";
    public string Address { get; set; } = "";
    public string Type { get; set; } = "Client"; // Client or Vendor
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
