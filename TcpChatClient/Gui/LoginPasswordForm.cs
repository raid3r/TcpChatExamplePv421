using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpChatClient.Gui;

public class LoginPasswordForm
{
    public string Title { get; set; } = "Login Form";
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public void Show()
    {
        Console.Clear();
        Console.WriteLine(Title);
        Console.WriteLine(new string('-', 20));
        
        Console.Write("Enter your login: ");
        Login = Console.ReadLine() ?? string.Empty;
        
        Console.Write("Enter your password: ");
        Password = Console.ReadLine() ?? string.Empty;
    }
}
