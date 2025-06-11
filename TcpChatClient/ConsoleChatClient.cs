using ClassLibrary.ChatModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TcpChatClient;

public class ConsoleChatClient
{
    private MyChatClient chatClient;

    public ConsoleChatClient()
    {
        chatClient = new MyChatClient(IPAddress.Parse("127.0.0.1"), 5001);
    }

    private bool Login()
    {
        var loginForm = new Gui.LoginPasswordForm() { Title = "Login form" };
        loginForm.Show();

        try {
            var response = chatClient.Login(loginForm.Login, loginForm.Password);
            Console.WriteLine("Login successful!");
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
            return true;
        } catch (Exception ex) {
            Console.WriteLine($"Error during login: {ex.Message}");
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
            return false;
        }
    }

    private bool Register()
    {
        var registerForm = new Gui.LoginPasswordForm() { Title = "Register form" };
        registerForm.Show();
        try {
            var response = chatClient.Register(registerForm.Login, registerForm.Password);
            Console.WriteLine("Registration successful!");
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
            return true;
        } catch (Exception ex) {
            Console.WriteLine($"Error during registration: {ex.Message}");
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
            return false;
        }
    }


    public void Run()
    {
        while (true) {
            // Вибір вхід або реєстрація
            var select = new Gui.Select(new List<string> { "Login", "Register" }, "Select an option");
            var choice = select.Show();

            switch (choice)
            {
                case "Login":
                    if (!Login()) { 
                        continue; // Якщо логін не вдався, повторити спробу
                    }
                    break;
                case "Register":
                    if (!Register()) {
                        continue; // Якщо реєстрація не вдалась, повторити спробу
                    }
                    break;
                default:
                    Console.WriteLine("Invalid choice. Exiting.");
                    return;
            }

            // Після успішного логіну або реєстрації
            Console.Clear();
            
            Task.Run(async () => {
                var lastMessageId = 0; // Ідентифікатор останнього отриманого повідомлення
                List<ChatMessage> messages = [];

                while (true) {
                    if (messages.Count > 0)
                    {
                        lastMessageId = messages.Max(m => m.MessageId); // Оновити останній ідентифікатор повідомлення
                    }
                    var receivedMessages = chatClient.GetMessages(lastMessageId);
                    receivedMessages.ForEach(msg => messages.Add(msg)); // Додати нові повідомлення до списку

                    var last30Messages = messages.TakeLast(30).Reverse().ToList();

                    
                    for (int i = 0; i < last30Messages.Count; i++)
                    {
                        lock(Console.Out) // Забезпечити потокобезпечний доступ до консолі
                        {
                            Console.SetCursorPosition(0, 2 + i);
                            Console.WriteLine(new string(' ', Console.WindowWidth)); // Очистити рядок
                            var msg = last30Messages[i];
                            Console.SetCursorPosition(0, 2 + i);
                            Console.WriteLine($"{msg.Timestamp} {msg.Sender.Login} : {msg.Text} ");
                        }
                    }
        
                    Console.SetCursorPosition(0, 1);
                    await Task.Delay(3000); // Оновлювати кожні 3 секунди
                }
            });

            while (true)
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("Welcome to the chat! Type your messages below. Type 'exit' to quit.");
                Console.SetCursorPosition(0, 1);
                var message = Console.ReadLine();
                
                if (message?.ToLower() == "exit")
                {
                    Console.WriteLine("Exiting chat. Goodbye!");
                    return;
                }
                if (!string.IsNullOrWhiteSpace(message))
                {
                    try {
                        chatClient.SendMessage(message);
                        Console.SetCursorPosition(0, 1);
                        Console.WriteLine($"                                                                   ");
                    } catch (Exception ex) {
                        Console.WriteLine($"Error sending message: {ex.Message}");
                    }
                }

            }

        }
    }
}
