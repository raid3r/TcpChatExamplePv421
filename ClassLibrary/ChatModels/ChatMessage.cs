using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.ChatModels;

public class ChatMessage
{
    public int MessageId { get; set; } // Унікальний ідентифікатор повідомлення
    public string Text { get; set; } = string.Empty; // Текст повідомлення
    public DateTime Timestamp { get; set; } // Час відправлення повідомлення
    public ChatUser Sender { get; set; }  // Користувач, який надіслав повідомлення

}
