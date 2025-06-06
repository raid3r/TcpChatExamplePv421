using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpChatServer.Models;

public class Message
{
    public int MessageId { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }
    public User From { get; set; }
}
