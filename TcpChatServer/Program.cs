using ClassLibrary;
using System.Net;
using System.Net.Sockets;
using System.Text;

Console.WriteLine("Server");


int port = 5001;
IPAddress address = IPAddress.Parse("127.0.0.1");

TcpListener server = new TcpListener(address, port);
server.Start();
Console.WriteLine($"Server started on {address}:{port}");


while (true)
{
    TcpClient client = server.AcceptTcpClient();
    Console.WriteLine("Client connected");

    Thread clientThread = new Thread(() =>
    {
        NetworkStream stream = client.GetStream();
        Console.WriteLine("Sent welcome message to client");
        MyNetworkHerper.SendDataBlock(stream, System.Text.Encoding.UTF8.GetBytes("Welcome to the chat server!"));
        


        var message = Encoding.UTF8.GetString(MyNetworkHerper.GetDataBlock(stream));
        Console.WriteLine($"Received message: {message}");

        Console.WriteLine("Client disconnected");
        client.Close();
    });
    clientThread.Start();


}


/*
 * Чат без постійного з'єднання
 * 
 * 
 * 
 * 
 */ 