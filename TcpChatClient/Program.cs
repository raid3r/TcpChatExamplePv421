using ClassLibrary;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TcpChatClient;


Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;

var consoleClient = new ConsoleChatClient();
consoleClient.Run();
