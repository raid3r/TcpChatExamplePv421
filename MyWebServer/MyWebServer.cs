using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TcpWebServer;

// Клас простого веб-сервера
public class MyWebServer
{
    // Внутрішній клас для представлення HTTP-запиту
    class HttpRequest
    {
        public string Method { get; set; } // Метод запиту (GET, POST і т.д.)
        public string Path { get; set; } // Шлях запиту (/index.html)
        public string HttpVersion { get; set; } // Версія HTTP (HTTP/1.1)
        public Dictionary<string, string> Headers { get; set; } = []; // Заголовки запиту
        public string Body { get; set; } = string.Empty; // Тіло запиту
    }

    // Зчитування та розбір першого рядка HTTP-запиту
    HttpRequest GetRequest(StreamReader reader)
    {
        // Зчитуємо перший рядок запиту
        string? requestLine = reader.ReadLine();
        // Розбиваємо рядок на метод, шлях і версію HTTP
        var request = new HttpRequest()
        {
            Method = requestLine?.Split(' ')[0] ?? string.Empty, // GET
            Path = requestLine?.Split(' ')[1] ?? string.Empty, // /about
            HttpVersion = requestLine?.Split(' ')[2] ?? "HTTP/1.1" // HTTP/1.1
        };
        return request;
    }

    // Визначення MIME-типу за розширенням файлу
    private string GetMimeTypeByFilename(string filename)
    {
        string mimeType = "application/octet-stream"; // Тип за замовчуванням
        var extension = Path.GetExtension(filename).ToLowerInvariant();
        switch (extension)
        {
            case ".html":
            case ".htm":
                mimeType = "text/html";
                break;
            case ".css":
                mimeType = "text/css";
                break;
            case ".js":
                mimeType = "application/javascript";
                break;
            case ".jpg":
            case ".jpeg":
                mimeType = "image/jpeg";
                break;
            case ".png":
                mimeType = "image/png";
                break;
            case ".gif":
                mimeType = "image/gif";
                break;
            case ".json":
                mimeType = "application/json";
                break;
            default:
                mimeType = "application/octet-stream"; // Тип за замовчуванням
                break;
        }
        return mimeType;
    }

    // Отримання текстового опису коду відповіді HTTP
    private string GetCodeDescription(int code)
    {
        return code switch
        {
            200 => "OK",
            404 => "Not Found",
            500 => "Internal Server Error",
            400 => "Bad Request",
            403 => "Forbidden",
            401 => "Unauthorized",
            408 => "Request Timeout",
            501 => "Not Implemented",
            502 => "Bad Gateway",
            // ...
            _ => "Unknown"
        };
    }

    // Відправка файлу як HTTP-відповіді
    private void SendFileResponse(NetworkStream stream, string requestFilePath, int code = 200)
    {
        // Зчитуємо вміст файлу
        var fileContent = File.ReadAllBytes(requestFilePath);
        // Визначаємо MIME-тип
        string mimeType = GetMimeTypeByFilename(requestFilePath);
        // Формуємо заголовок HTTP-відповіді
        string header =
            $"HTTP/1.1 {code} {GetCodeDescription(code)}\r\n" +
            "Server: CustomServer/1.0\r\n" +
            $"Content-Type: {mimeType}\r\n" +
            $"Content-Length: {fileContent.Length}\r\n" +
            "Connection: close\r\n\r\n";
        // Відправляємо заголовок
        stream.Write(Encoding.UTF8.GetBytes(header));
        // Відправляємо тіло (файл)
        stream.Write(fileContent);
        stream.Flush();
    }

    // Основний метод обробки HTTP-запиту
    public void HandleRequest(NetworkStream stream)
    {
        using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
        var request = GetRequest(reader);

        Console.WriteLine($"{request.Method} {request.Path}");

        // Шлях до папки з веб-ресурсами
        var wwwPath = "C:\\Users\\kvvkv\\source\\repos\\TcpChat\\TcpChatServer\\wwwroot\\";
        //var wwwPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        int code = 200;

        // Якщо запитаний корінь, віддаємо index.html
        if (request.Path == "/")
        {
            request.Path = "/index.html"; // Головна сторінка
        }

        // Формуємо повний шлях до файлу
        var requestFilePath = Path.Combine(wwwPath, request.Path.TrimStart('/'));
        // Якщо файл не існує, повертаємо сторінку 404
        if (!File.Exists(requestFilePath))
        {
            requestFilePath = Path.Combine(wwwPath, "error404.html"); // Файл не знайдено, повертаємо 404 сторінку
            code = 404; // Код відповіді 404 Not Found
        }

        Console.WriteLine($"Response code: {code}: {requestFilePath}");
        // Відправляємо відповідь
        SendFileResponse(stream, requestFilePath, code);
    }
}


/*
 * 
 * Зробіть свій сайт - з статичними сторінками 
 * Додайте свої стилі
 * Зробіть сторінки красивими
 * А також додайте якийсь фунціонал на сторінках з використанням javascript
 * 
 * 
 */

