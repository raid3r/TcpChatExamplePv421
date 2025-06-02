using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TcpChatServer;

public class MyWebServer
{
    class HttpRequest
    {
        public string Method { get; set; }
        public string Path { get; set; }
        public string HttpVersion { get; set; }
        public Dictionary<string, string> Headers { get; set; } = new();
        public string Body { get; set; } = string.Empty;
    }

    HttpRequest GetRequest(StreamReader reader)
    {
        // Читач даних
        string? requestLine = reader.ReadLine();
        Console.WriteLine($"{requestLine}");
        // Треба отримати метод запиту, шлях та версію HTTP
        var request = new HttpRequest()
        {
            Method = requestLine?.Split(' ')[0] ?? string.Empty, // GET
            Path = requestLine?.Split(' ')[1] ?? string.Empty, // /about
            HttpVersion = requestLine?.Split(' ')[2] ?? "HTTP/1.1" // HTTP/1.1
        };
        return request;
    }

    string NotFoundErrorResponse()
    {
        string htmlBody = """
<!DOCTYPE html>
<html>
<head><title>My web site</title></head>
<body>
    <h1>NOT FOUND</h1>
    <p>Не знайдено</p>
<p><a href="/">Main page</a></p>
</body>
</html>
""";

        string httpResponse =
            "HTTP/1.1 404 NOT FOUND\r\n" +
            "Date: Mon, 02 Jun 2025 20:11:00 GMT\r\n" +
            "Server: CustomServer/1.0\r\n" +
            "Content-Type: text/html; charset=UTF-8\r\n" +
            $"Content-Length: {Encoding.UTF8.GetByteCount(htmlBody)}\r\n" +
            "Connection: close\r\n" +
            "\r\n" + // Пустая строка между заголовками и телом
            htmlBody;

        return httpResponse;
    }


    string MainPageResponse()
    {
        string htmlBody = """
<!DOCTYPE html>
<html>
<head><title>My web site</title></head>
<body>
    <h1>Main page</h1>
    <p><a href="/about">About me</a></p>
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

        return httpResponse;
    }

    public void HandleRequest(NetworkStream stream)
    {
        // Читач даних
        using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
        using StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
        var request = GetRequest(reader);
        
        if (request.Path == "/about")
        {
            
            
        }

        if (request.Path == "/")
        {
            writer.Write(MainPageResponse());
            writer.Flush();
            return;
        }

        writer.Write(NotFoundErrorResponse());
        writer.Flush();
        return;


        if (request.Path == "/photo.jpeg")
        {
            
        }



        // Формуємо відповідь сервера в форматі json
        var data = new { Message = "Hello world" };
        string jsonBody = JsonSerializer.Serialize(data);


        // Формуємо відповідь сервера в вигляді HTML-сторінки





        //writer.Write(jsonResponse);
        
    }


}
