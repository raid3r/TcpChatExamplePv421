using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.ChatModels;

public class GetMessagesResponse
{
    public List<ChatMessage> Messages { get; set; } = []; // Список повідомлень

}