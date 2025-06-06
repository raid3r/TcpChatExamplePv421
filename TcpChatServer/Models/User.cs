using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpChatServer.Models;

public class User
{
    public int UserId { get; set; }
    [MaxLength(100)]
    public string Login { get; set; } = string.Empty;
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; } = null;

    public string AuthToken { get; set; } = string.Empty; // Токен для авторизації
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>(); // Пов'язані повідомлення
}
