using ClassLibrary;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TcpChatClient;



var chatClient = new MyChatClient(IPAddress.Parse("127.0.0.1"), 5001);

var login = "user1";
var password = "password";

chatClient.Register(login, password);

Console.ReadLine();

