using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.ChatModels;

public class RegisterResponse
{
    public int UserId { get; set; }
    public string AuthToken { get; set; } = string.Empty; // Токен авторизації
}
