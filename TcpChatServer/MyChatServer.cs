﻿using ClassLibrary.ChatModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TcpChatServer.Models;
using BCrypt.Net;
using System.Runtime.ExceptionServices;
using Microsoft.EntityFrameworkCore;

namespace TcpChatServer;

public class MyChatServer
{
    private ClientRequest ParseRequest(NetworkStream stream)
    {
        StreamReader reader = new StreamReader(stream, Encoding.UTF8);

        var method = reader.ReadLine();
        var authorization = reader.ReadLine();
        var body = reader.ReadLine();

        /**
        * Запит:
        * /register  - метод
        * Authorization: N/A
        * {"Login": "User1", "Password": "password"}
        **/

        return new ClientRequest()
        {
            Method = method.Trim() ?? string.Empty,
            Body = body.Trim(),
            Authorization = authorization?.Replace("Authorization: ", string.Empty).Trim(),
        };
    }

    private void SendResponse(NetworkStream stream, string status, string body)
    {
        Console.WriteLine($"Response: {status}");
        Console.WriteLine($"Body: {body}");
        using StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
        writer.WriteLine(status);
        writer.WriteLine(body);
        writer.Flush();
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }

    private void HandleRegister(ClientRequest request, NetworkStream stream)
    {
        var reqisterRequest = JsonSerializer.Deserialize<RegisterRequest>(request.Body);

        if (reqisterRequest == null || string.IsNullOrEmpty(reqisterRequest.Login) || string.IsNullOrEmpty(reqisterRequest.Password))
        {
            SendResponse(stream,
                ResponseStatus.ERROR,
                JsonSerializer.Serialize(new ErrorResponse { Message = "Invalid request body" })
                );
            return;
        }

        // Перевірити чи користувач з таким логіном вже існує

        using (var dbContext = new ChatContext())
        {
            var userExists = dbContext.Users.Any(u => u.Login == reqisterRequest.Login);
            // Якщо існує, то повернути помилку
            if (userExists)
            {
                SendResponse(stream,
                    ResponseStatus.ERROR,
                    JsonSerializer.Serialize(
                        new ErrorResponse { Message = "User already exists" })
                    );
                return;
            }


            // Якщо не існує, то створити нового користувача і повернути успішний результат з токеном
            var user = new User
            {
                Login = reqisterRequest.Login,
                PasswordHash = HashPassword(reqisterRequest.Password),
                CreatedAt = DateTime.Now,

                AuthToken = Guid.NewGuid().ToString() // Тут має бути логіка генерації токена
            };
            dbContext.Users.Add(user);
            dbContext.SaveChanges(); // Зберегти користувача в базі даних


            SendResponse(stream,
                ResponseStatus.OK,
                JsonSerializer.Serialize(
                    new RegisterResponse
                    {
                        UserId = user.UserId,
                        AuthToken = user.AuthToken
                    })
                );
        }
    }


    private void HandleLogin(ClientRequest request, NetworkStream stream)
    {
        var loginRequest = JsonSerializer.Deserialize<LoginRequest>(request.Body);

        if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Login) || string.IsNullOrEmpty(loginRequest.Password))
        {
            SendResponse(stream,
                ResponseStatus.ERROR,
                JsonSerializer.Serialize(new ErrorResponse { Message = "Invalid request body" })
                );
            return;
        }

        // Перевірити чи користувач з таким логіном вже існує

        using (var dbContext = new ChatContext())
        {
            var user = dbContext.Users.FirstOrDefault(u => u.Login == loginRequest.Login);

            // Якщо не існує, то повернути помилку
            if (user == null)
            {
                SendResponse(stream,
                    ResponseStatus.ERROR,
                    JsonSerializer.Serialize(
                        new ErrorResponse { Message = "User not exists" })
                    );
                return;
            }

            if (!VerifyPassword(loginRequest.Password, user.PasswordHash))
            {
                SendResponse(stream,
                    ResponseStatus.ERROR,
                    JsonSerializer.Serialize(
                        new ErrorResponse { Message = "Invalid password" })
                    );
                return;
            }

            user.AuthToken = Guid.NewGuid().ToString();
            dbContext.SaveChanges(); // Зберегти користувача в базі даних


            SendResponse(stream,
                ResponseStatus.OK,
                JsonSerializer.Serialize(
                    new LoginResponse
                    {
                        UserId = user.UserId,
                        AuthToken = user.AuthToken
                    })
                );
        }
    }


    private void HandleGetUsers(ClientRequest request, NetworkStream stream)
    {
        var getUsersRequest = JsonSerializer.Deserialize<GetUsersRequest>(request.Body);

        var authToken = request.Authorization;
        if (string.IsNullOrEmpty(authToken))
        {
            SendResponse(stream,
                ResponseStatus.ERROR,
                JsonSerializer.Serialize(new ErrorResponse { Message = "Authorization token is required" })
                );
            return;
        }

        // Перевірити чи користувач з таким токеном існує
        using (var dbContext = new ChatContext())
        {
            var user = dbContext.Users.FirstOrDefault(u => u.AuthToken == authToken);
            // Якщо не існує, то повернути помилку
            if (user == null)
            {
                SendResponse(stream,
                    ResponseStatus.ERROR,
                    JsonSerializer.Serialize(
                        new ErrorResponse { Message = "Invalid authorization token" })
                    );
                return;
            }

            SendResponse(stream,
                ResponseStatus.OK,
                JsonSerializer.Serialize(
                    new GetUsersResponse
                    {
                        Users = [..dbContext.Users
                        .Select(u => new ChatUser
                        {
                            UserId = u.UserId,
                            Login = u.Login
                        })]
                    }
                    )
                );
        }


    }
    public void HandleSendMessage(ClientRequest request, NetworkStream stream)
    {
        var sendMessageRequest = JsonSerializer.Deserialize<SendMessageRequest>(request.Body);
        var authToken = request.Authorization;
        if (string.IsNullOrEmpty(authToken))
        {
            SendResponse(stream,
                ResponseStatus.ERROR,
                JsonSerializer.Serialize(new ErrorResponse { Message = "Authorization token is required" })
                );
            return;
        }
        // Перевірити чи користувач з таким токеном існує
        using (var dbContext = new ChatContext())
        {
            var user = dbContext.Users.FirstOrDefault(u => u.AuthToken == authToken);
            // Якщо не існує, то повернути помилку
            if (user == null)
            {
                SendResponse(stream,
                    ResponseStatus.ERROR,
                    JsonSerializer.Serialize(
                        new ErrorResponse { Message = "Invalid authorization token" })
                    );
                return;
            }
            // Додати повідомлення в базу даних
            var message = new Message
            {
                From = user,
                Text = sendMessageRequest.Text,
                CreatedAt = DateTime.Now
            };
            dbContext.Messages.Add(message);
            dbContext.SaveChanges(); // Зберегти повідомлення в базі даних
            SendResponse(stream,
                ResponseStatus.OK,
                JsonSerializer.Serialize(
                    new SendMessageResponse
                    {
                        MessageId = message.MessageId
                    })
                );
        }
    }
    public void HandleGetMessage(ClientRequest request, NetworkStream stream)
    {
        var getMessagesRequest = JsonSerializer.Deserialize<GetMessagesRequest>(request.Body);
        var authToken = request.Authorization;
        if (string.IsNullOrEmpty(authToken))
        {
            SendResponse(stream,
                ResponseStatus.ERROR,
                JsonSerializer.Serialize(new ErrorResponse { Message = "Authorization token is required" })
                );
            return;
        }
        // Перевірити чи користувач з таким токеном існує
        using (var dbContext = new ChatContext())
        {
            var user = dbContext.Users.FirstOrDefault(u => u.AuthToken == authToken);
            // Якщо не існує, то повернути помилку
            if (user == null)
            {
                SendResponse(stream,
                    ResponseStatus.ERROR,
                    JsonSerializer.Serialize(
                        new ErrorResponse { Message = "Invalid authorization token" })
                    );
                return;
            }
            // Отримати повідомлення з бази даних
            var messagesQuery = dbContext.Messages.Include(x=> x.From).AsQueryable();
             messagesQuery = messagesQuery.Where(m => m.MessageId > getMessagesRequest.LastMessageId);
            
             SendResponse(stream,
                ResponseStatus.OK,
                JsonSerializer.Serialize(
                    new GetMessagesResponse
                    {
                        Messages = [..messagesQuery
                        .Select(u => new ChatMessage
                        {
                            MessageId = u.MessageId,
                            Sender = new ChatUser
                            {
                                UserId = u.From.UserId,
                                Login = u.From.Login
                            },
                            Text = u.Text,
                            Timestamp = u.CreatedAt
                        })
                        ]
                    }
                    )
                );
        }
    }
    public void HandleRequest(NetworkStream stream)
    {
        // Отримати запит від клієнта
        ClientRequest request = ParseRequest(stream);

        Console.WriteLine("Request");
        Console.WriteLine($"{request.Method}\n{request.Authorization}\n{request.Body}");

        // Зрозуміти що в запиті
        // Сформувати відповідь
        // Відправити відповідь
        switch (request.Method)
        {
            case RequestType.Register:
                HandleRegister(request, stream);
                break;
            case RequestType.Login:
                HandleLogin(request, stream);
                break;
            case RequestType.GetUsers:
                HandleGetUsers(request, stream);
                break;
            case RequestType.SendMessage:
                HandleSendMessage(request, stream);
                break;
            case RequestType.GetMessages:
                HandleGetMessage(request, stream);
                break;

            default:
                SendResponse(stream,
                    ResponseStatus.ERROR,
                    JsonSerializer.Serialize(new ErrorResponse { Message = "Unknown request" })
                    );
                break;
        }






    }
}

/*
* Чат на TCP протоколі
* 
* Сервер з базою даних користувачів та повідомлень
* 
* Слухає TCP порт
* Приймає з'єднання від клієнта
*
* Реєстрація користувача
* - Клієнт відправляє запит на реєстрацію з логіном та паролем
* - Сервер перевіряє чи користувач з таким логіном вже існує
* - Якщо існує, то повертає помилку
* - Якщо не існує, то створює нового користувача і повертає успішний результат з токеном
*
* Запит:
* /register
* Authorization: N/A
* {"Login": "User1", "Password": "password"}
* Відповідь:
* Успіх
* OK
* {UserId:123, "AuthToken": "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"}
* 
* Помилка
* ERROR
* {"Message": "Error description"}
* 
*
*
* Авторизація користувача
* - Клієнт відправляє запит на авторизацію з логіном та паролем
* - Сервер перевіряє чи користувач з таким логіном та паролем існує
* - Якщо існує, то повертає успішний результат з токеном
* 
* * Запит:
* /login
* Authorization: N/A
* {"Login": "User1", "Password": "password"}
* Відповідь:
* Успіх
* OK
* {UserId:123, "AuthToken": "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"}
* 
* Помилка
* ERROR
* {"Message": "Error description"}
* 
* 
* 
* Отримання нового повідомлення (з токеном)
*  - Отримує повідомлення від клієнта в форматі JSON
*  - Додає повідомлення в базу даних
*  - Відповідає клієнту що повідомлення прийнято
* 
* * Запит:
* /send-message
* Authorization: XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
* {"Text": "Hello world", "To": {"UserId": 456}}
* Відповідь:
* Успіх
* OK
* {"MessageId": 123}
* 
* Помилка
* ERROR
* {"Message": "Error description"}
* 
* 
* Отримання повідомлень для клієнта (з токеном)
* - Клієнт запитує повідомлення
* - Сервер відправляє всі повідомлення, які були отримані з моменту останнього запиту клієнта
* - Якщо повідомлень немає, то сервер відправляє порожній масив
* 
* Запит:
* /get-messages
* Authorization: XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
* {"LastMessageId": 123}
* Відповідь:
* Успіх
* OK
* [{"MessageId": 123, "From": {"UserId": 1}, "To": {"UserId": 2}, "Text": "Hello world"}, "CreateAt": "2025-06-06 18:55:00"]
* 
* Помилка
* ERROR
* {"Message": "Error description"}
* 
* 
* Отримати список користувачів (з токеном)
* - Клієнт запитує список користувачів
* - Сервер відправляє список користувачів, які зареєстровані в системі
* - Якщо користувачів немає, то сервер відправляє порожній масив
* 
* Запит:
* /get-users
* Authorization: XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
* {"????"}
* Відповідь:
* Успіх
* OK
* [{"UserId": 1, "Login": "User1"}, {"UserId": 2, "Login": "User2"}]
* 
* Помилка
* ERROR
* {"Message": "Error description"} 
* Клієнт
* 
* При запуску програми запитує зареєстуватися чи увійти в систему
* Запишує логін та пароль
* Передає запит на реєстрацію чи авторизацію на сервер
* Якщо успішно, то отримує токен
* З токеном запитує список користувачів та повідомлень
* Повторює це кожні 5 секунд
* 
* 
* 
* 
* 
* 
* 
*/ 