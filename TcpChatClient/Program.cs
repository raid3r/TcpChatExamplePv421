using ClassLibrary;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TcpChatClient;



var chatClient = new MyChatClient(IPAddress.Parse("127.0.0.1"), 5001);
//var login = "user1";
//var password = "password";

Console.WriteLine("Chat Client");
Console.WriteLine("1- Login, 2 - Register");
var choice = Console.ReadLine();
if (choice != "1" && choice != "2")
{
    Console.WriteLine("Invalid choice. Exiting.");
    return;
}

Console.Write("Login: ");
var login = Console.ReadLine();

Console.Write("Password: ");
var password = Console.ReadLine();

if (choice == "1")
{
    chatClient.Login(login, password);
}
else
{
    chatClient.Register(login, password);
}

var users = chatClient.GetUsers();
users.ForEach(user => Console.WriteLine($"Id: {user.UserId} login: {user.Login}"));




Console.ReadLine();

