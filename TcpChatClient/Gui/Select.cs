using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpChatClient.Gui;

public class Select
{
    private List<string> Options { get; set; }
    private string Title { get; set; } = "Select an option";

    private int SelectedIndex { get; set; } = 0;

    public Select(List<string> options, string title = "Select an option")
    {
        Options = options;
        Title = title;
    }

    public string Show()
    {
        Console.Clear();
        Console.WriteLine(Title);
        Console.WriteLine(new string('-', Title.Length));
        
        for (int i = 0; i < Options.Count; i++)
        {
            if (i == SelectedIndex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"> {Options[i]} <");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"  {Options[i]}  ");
            }
        }
        while (true)
        {
            var key = Console.ReadKey(true).Key;
            switch (key)             {
                case ConsoleKey.UpArrow:
                    SelectedIndex = (SelectedIndex - 1 + Options.Count) % Options.Count;
                    break;
                case ConsoleKey.DownArrow:
                    SelectedIndex = (SelectedIndex + 1) % Options.Count;
                    break;
                case ConsoleKey.Enter:
                    return Options[SelectedIndex];
            }
            
            Show(); // Refresh the display
        }
    }

}
