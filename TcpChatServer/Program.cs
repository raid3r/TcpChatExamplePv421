using ClassLibrary;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using TcpChatServer;

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


/*
 * Доробити свій веб сервер
 * 
 * Головна сторінка / - коротка інфрормація про цей проєкт та посилання на сторінку /about
 * Сторінка інформація про вас /about з вашим фото
 * Картинка (ваше фото) - адреса 127.0.0.1/photo.jpeg
 * 
 * 
 */


/*
 * Чат без постійного з'єднання
 * 
 * 
 * 
 * З'єднання клієнта з сервером
 * Клієнт надсилає запит серверу
 * Сервер відповідає клієнту
 * З'єднання клієнта з сервером закривається
 * 
 * 
 * 
 */ 