using PainKiller.PowerCommands.ReadLine;
using PainKiller.PowerCommands.Security.Services;

namespace PainKiller.PowerCommands.Core.Services;
public static class DialogService
{
    public static bool YesNoDialog(string question, string yesValue = "y", string noValue = "n")
    {
        WriteHeader($"\n{question}"); ;
        Console.WriteLine($"({yesValue}/{noValue}):");
        
        var response = Console.ReadLine();
        return $"{response}".Trim().ToLower() == yesValue.ToLower();
    }
    public static string QuestionAnswerDialog(string question)
    {
        WriteHeader($"{question}\n");
        Console.Write(ConfigurationGlobals.Prompt);
        var response = Console.ReadLine();
        return $"{response}".Trim();
    }
    public static string SecretPromptDialog(string question, int maxRetries = 3)
    {
        var retryCount = 0;
        var secret = "";
        while (retryCount < maxRetries)
        {
            WriteHeader($"\n{question} ");
            secret = PasswordPromptService.Service.ReadPassword();
            Console.WriteLine();
            Console.Write("Confirm: ".PadLeft(question.Length + 1));
            var confirm = PasswordPromptService.Service.ReadPassword();
            if (secret != confirm)
            {
                ConsoleService.Service.WriteCritical(nameof(DialogService), "\nConfirmation failure, please try again.\n");
                retryCount++;
            }
            else break;
        }

        return $"{secret}".Trim();
    }

    public static Dictionary<int, string> ListDialog(string header, List<string> items, bool multiSelect = false, bool autoSelectIfOnlyOneItem = true, ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Blue, bool clearConsole = true)
    {
        if (items.Count == 0) return new Dictionary<int, string>();
        if (items.Count - 4 > Console.WindowHeight)
        {
            clearConsole = true;
            var quit = false;
            var currentPage = 0;
            var pageSize = Console.WindowHeight - 4;
            while (!quit)
            {
                var pageItems = items.Skip(currentPage * pageSize).Take(pageSize).ToList();

                var response = ListDialogPage(header, pageItems, multiSelect, autoSelectIfOnlyOneItem, foregroundColor, backgroundColor, clearConsole, currentPage + 1);
                if(response.Count == 0) { return new(); }
                if (response.Count > 1) return response.AdjustToCurrentPage(currentPage, pageSize);
                if (response.First().Key < 0)
                {
                    if (response.First().Value == "n")
                    {
                        if (currentPage * pageSize < items.Count-pageSize) currentPage++;
                        continue;
                    }
                    if (currentPage > 0) currentPage--;
                }
                else
                {
                    return response.AdjustToCurrentPage(currentPage, pageSize);;
                }
            }
        }
        return ListDialogPage(header, items, multiSelect, autoSelectIfOnlyOneItem, foregroundColor, backgroundColor, clearConsole);
    }

    private static Dictionary<int, string>  ListDialogPage(string header, List<string> items, bool multiSelect = false, bool autoSelectIfOnlyOneItem = true, ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Blue, bool clearConsole = true, int currentPage = 0)
    {
        
        if(clearConsole) Console.Clear();
        WriteHeader($"{header}\n");
        var startRow = Console.CursorTop;
        var startForegroundColor = Console.ForegroundColor;
        var startBackgroundColor = Console.BackgroundColor;

        for (int index=0; index<items.Count;index++)
        {
            var item = items[index];
            Console.WriteLine($"{index+1}. {item}");
        }
        var quit = " ";
        var input = "";
        var selectedItems = new Dictionary<int, string>();

        if (items.Count == 1 && autoSelectIfOnlyOneItem)
        {
            selectedItems.Add(0, items.First());
            Console.WriteLine("");
            Console.Write(" ");
            ConsoleService.Service.WriteRowWithColor(3, foregroundColor, backgroundColor, $" 1. {items.First()}");
            return selectedItems;
        }

        var label = multiSelect ? "Enter number(s) and hit enter or just enter to quit" : "Enter a number and hit enter or just enter to quit";
        if (currentPage > 0) label += $"\nCurrent page is ({currentPage}) Enter (n) for next page (p) for previous page.";
        while (input != quit)
        {
            Console.WriteLine("");
            Console.Write($"{label}>");
            input = ReadLineService.Service.Read();
            if(input.Trim() == "") break;
            if(currentPage > 0 && (input.Trim() == "n" || input.Trim() == "p")) return new Dictionary<int, string>(){{-1, input.Trim()}};
            var selectedIndex = (int.TryParse(input, out var index) ? index : 1);
            if(selectedIndex > items.Count || selectedIndex < 1) selectedIndex = 1;
            var selectedItem = new { Index = selectedIndex, Value = items[selectedIndex - 1] };
            var itemAdded = selectedItems.TryAdd(selectedItem.Index-1, selectedItem.Value);
            
            ConsoleService.Service.ClearRow(Console.CursorTop-1);
            var top = Console.CursorTop - 2;
            Console.CursorTop = Math.Clamp(top, 0, Console.LargestWindowHeight - 1);
            Console.CursorLeft = 0;
            if (currentPage > 0) startRow--;
            if(itemAdded) ConsoleService.Service.WriteRowWithColor(startRow + selectedIndex - 1, foregroundColor, backgroundColor, $"{selectedIndex}. {selectedItem.Value}");
            else
            {
                selectedItems.Remove(selectedItem.Index);
                ConsoleService.Service.WriteRowWithColor(startRow + selectedIndex - 1, startForegroundColor, startBackgroundColor, $"{selectedIndex}. {selectedItem.Value}");
            }

            if (!multiSelect) break;
        }
        ToolbarService.ClearToolbar();
        return selectedItems;
    }
    private static Dictionary<int, string> AdjustToCurrentPage(this Dictionary<int, string> items, int currentPage, int pageSize) => items.ToDictionary(item => item.Key + (currentPage * pageSize), item => item.Value);
    private static void WriteHeader(string text)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(text);
        Console.ForegroundColor = originalColor;
    }
}