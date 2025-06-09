using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.ChatModels;

public class ChatUser
{
    public int UserId { get; set; } // Унікальний ідентифікатор користувача
    public string Login { get; set; } = string.Empty; // Ім'я користувача
}
