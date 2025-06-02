using ClassLibrary;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

Console.WriteLine("Client");


int port = 5001;
IPAddress address = IPAddress.Parse("127.0.0.1");

var client = new TcpClient();

client.Connect(address, port);

Console.WriteLine($"Connected to server at {address}:{port}");

var serverData = MyNetworkHerper.GetDataBlock(client.GetStream());
var serverMessage = Encoding.UTF8.GetString(serverData);
Console.WriteLine($"Received message from server: {serverMessage}");

var message = "Hello, server!";
NetworkStream stream = client.GetStream();
MyNetworkHerper.SendDataBlock(stream, System.Text.Encoding.UTF8.GetBytes(message));
Console.WriteLine($"Sent message to server: {message}");

Console.ReadLine();

