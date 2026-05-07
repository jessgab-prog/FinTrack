using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinTrack.Models;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = "Owner"; // Owner or Staff
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}   
