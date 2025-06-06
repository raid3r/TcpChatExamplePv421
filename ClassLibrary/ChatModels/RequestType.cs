using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.ChatModels;

public static class RequestType
{
    public const string
           Register = "/register",
           Login = "/login",
           SendMessage = "/send-message",
           GetMessages = "/get-messages",
           GetUsers = "/get-users";
}
