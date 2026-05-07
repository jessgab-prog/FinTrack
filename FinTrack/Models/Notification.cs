using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinTrack.Models;

public class Notification
{
    public int Id { get; set; }
    public string Message { get; set; } = "";
    public string Type { get; set; } = "Info"; // Info, Warning, Danger, Success
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}