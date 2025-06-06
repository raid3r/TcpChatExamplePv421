using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.ChatModels;

public class ServerResponse
{
    public string Status { get; set; }
    public string Body { get; set; } = string.Empty;
}
