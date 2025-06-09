using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.ChatModels;

public class GetUsersResponse
{
    public List<ChatUser> Users { get; set; } = []; // Список користувачів
}
