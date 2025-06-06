using ClassLibrary;
using ClassLibrary.ChatModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TcpChatClient;

public class MyChatClient(IPAddress iPAddress, int port)
{

    TcpClient Connect()
    {
        // Тут реалізуйте логіку підключення до сервера
        // Наприклад, створення TCP-клієнта та підключення до сервера
        var client = new TcpClient();
        client.Connect(iPAddress, port);
        Console.WriteLine($"Connected to server at {iPAddress}:{port}");
        return client;
    }

    //var serverData = MyNetworkHerper.GetDataBlock(client.GetStream());
    //    var serverMessage = Encoding.UTF8.GetString(serverData);
    //    Console.WriteLine($"Received message from server: {serverMessage}");

    //var message = "Hello, server!";
    //    NetworkStream stream = client.GetStream();
    //    MyNetworkHerper.SendDataBlock(stream, System.Text.Encoding.UTF8.GetBytes(message));
    //Console.WriteLine($"Sent message to server: {message}");

    private void SendRequest(NetworkStream stream, ClientRequest clientRequest)
    {
        StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
        writer.WriteLine(clientRequest.Method);
        writer.WriteLine($"Authorization: {clientRequest.Authorization}");
        writer.WriteLine(clientRequest.Body);
        writer.Flush();
    }


    private ServerResponse ReadResponse(NetworkStream stream)
    {
        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
        var status = reader.ReadLine();
        var body = reader.ReadLine();
        return new ServerResponse
        {
            Status = status?.Trim() ?? string.Empty,
            Body = body
        };
    }


    public void Register(string login, string password)
    {
        // Тут реалізуйте логіку реєстрації користувача
        // Наприклад, відправка запиту на сервер з логіном та паролем
        var client = Connect();
        var stream = client.GetStream();

        SendRequest(stream,
            new ClientRequest
            {
                Method = RequestType.Register,
                Authorization = "N/A",
                Body = JsonSerializer.Serialize(
                    new RegisterRequest
                    {
                        Login = login,
                        Password = password
                    }),
            });
        // Читання відповіді від сервера
        var response = ReadResponse(stream);

        // Обробка відповіді
        if (response.Status == ResponseStatus.OK)
        {
            Console.WriteLine("Registration successful!");
            var registerResponse = JsonSerializer.Deserialize<RegisterResponse>(response.Body);
            if (registerResponse != null)
            {
                Console.WriteLine($"Token: {registerResponse.AuthToken}");
            }
            else
            {
                Console.WriteLine("Failed to parse registration response.");
            }
        }
        else
        {
            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(response.Body);
            Console.WriteLine($"Registration failed: {errorResponse?.Message}");
        }
        client.Close();
    }
}
