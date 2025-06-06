using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using TcpWebServer;

Console.WriteLine("Server");


int port = 5001;
IPAddress address = IPAddress.Parse("127.0.0.1"); // localhost

TcpListener server = new TcpListener(address, port);
server.Start();
Console.WriteLine($"Server started on {address}:{port}");

var myWebServer = new MyWebServer();


while (true)
{
    // Приймаємо з'єднання від клієнта
    TcpClient client = server.AcceptTcpClient();
    Console.WriteLine("Client connected");


    Thread clientThread = new Thread(() =>
    {

        // Поток обробки клієнта

        // Мережевий поток для роботи з клієнтом
        NetworkStream stream = client.GetStream();
        
        myWebServer.HandleRequest(stream);
        Console.WriteLine("Client disconnected");
        client.Close();

    });
    clientThread.Start();


}