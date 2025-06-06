using ClassLibrary.ChatModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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

    private void HandleRegister(ClientRequest request, NetworkStream stream)
    {

        var reqisterRequest = JsonSerializer.Deserialize<RegisterRequest>(request.Body);

        // Перевірити чи користувач з таким логіном вже існує
        var userExists = true; // Тут має бути логіка перевірки в базі даних
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


        SendResponse(stream,
            ResponseStatus.OK,
            JsonSerializer.Serialize(
                new RegisterResponse
                {
                    UserId = 123, // Тут має бути логіка створення користувача в базі даних
                    AuthToken = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX" // Тут має бути логіка генерації токена
                })
            );
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
        switch (request.Method) {
            case RequestType.Register:
                HandleRegister(request, stream);
                break;
            case RequestType.Login:
                // Тут має бути логіка авторизації користувача
                break;
            case RequestType.SendMessage:
                // Тут має бути логіка обробки відправки повідомлення
                break;
            case RequestType.GetMessages:
                // Тут має бути логіка отримання повідомлень
                break;
            case RequestType.GetUsers:
                // Тут має бути логіка отримання списку користувачів
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