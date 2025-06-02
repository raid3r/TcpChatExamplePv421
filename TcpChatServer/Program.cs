using ClassLibrary;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

Console.WriteLine("Server");


int port = 5001;
IPAddress address = IPAddress.Parse("127.0.0.1"); // localhost

TcpListener server = new TcpListener(address, port);
server.Start();
Console.WriteLine($"Server started on {address}:{port}");


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

        // Читач даних
        using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
        //
        using StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };


        string? requestLine = reader.ReadLine(); 
        // GET /about HTTP/1.1
        Console.WriteLine($"{requestLine}");

        Dictionary<string, string> headers = new();

        string? line;
        while (!string.IsNullOrEmpty(line = reader.ReadLine()))
        {
            Console.WriteLine(line);
            var parts = line.Split(":", 2);
            if (parts.Length == 2)
                headers[parts[0].Trim()] = parts[1].Trim();
        }

        // (опционально) Читаем тело запроса
        string? body = null;
        if (headers.TryGetValue("Content-Length", out string? lengthValue) && int.TryParse(lengthValue, out int contentLength))
        {
            char[] buffer = new char[contentLength];
            int read = reader.Read(buffer, 0, contentLength);
            body = new string(buffer, 0, read);

            Console.WriteLine("Тело запроса:");
            Console.WriteLine(body);
        }

        // Формуємо відповідь сервера в форматі json
        var data = new {Message = "Hello world"};
        string jsonBody = JsonSerializer.Serialize(data);


        // Формуємо відповідь сервера в вигляді HTML-сторінки

        string htmlBody = """
<!DOCTYPE html>
<html>
<head><title>Example Page</title></head>
<body>
    <h1>Hello world</h1>
    <p>Це наш власний web сервер</p>
</body>
</html>
""";

        string httpResponse =
            "HTTP/1.1 200 OK\r\n" +
            "Date: Mon, 02 Jun 2025 20:11:00 GMT\r\n" +
            "Server: CustomServer/1.0\r\n" +
            "Content-Type: text/html; charset=UTF-8\r\n" +
            $"Content-Length: {Encoding.UTF8.GetByteCount(htmlBody)}\r\n" +
            "Connection: close\r\n" +
            "\r\n" + // Пустая строка между заголовками и телом
            htmlBody;

        string jsonResponse =
           "HTTP/1.1 200 OK\r\n" +
           "Date: Mon, 02 Jun 2025 20:11:00 GMT\r\n" +
           "Server: CustomServer/1.0\r\n" +
           "Content-Type: application/json; charset=UTF-8\r\n" +
           $"Content-Length: {Encoding.UTF8.GetByteCount(jsonBody)}\r\n" +
           "Connection: close\r\n" +
           "\r\n" + // Пустая строка между заголовками и телом
           jsonBody;

        //writer.Write(httpResponse);
        writer.Write(jsonResponse);
        writer.Flush();

        //var message = Encoding.UTF8.GetString(MyNetworkHerper.GetDataBlock(stream));
        //Console.WriteLine($"Received message: {message}");


        //Console.WriteLine("Sent welcome message to client");
        //MyNetworkHerper.SendDataBlock(stream, System.Text.Encoding.UTF8.GetBytes("Welcome to the chat server!"));



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