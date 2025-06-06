using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.ChatModels;

public class ClientRequest
{
    public string Method { get; set; } = string.Empty;
    public string? Authorization { get; set; } = null;
    public string Body { get; set; } = string.Empty;
}
