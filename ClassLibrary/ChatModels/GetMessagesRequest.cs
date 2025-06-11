using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.ChatModels;

public class GetMessagesRequest
{
    public int LastMessageId { get; set; } = 0;
}
