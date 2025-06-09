using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.ChatModels;

public class LoginRequest
{
    public string Login { get; set; } = string.Empty; // Логін користувача
    public string Password { get; set; } = string.Empty; // Пароль користувача
}
