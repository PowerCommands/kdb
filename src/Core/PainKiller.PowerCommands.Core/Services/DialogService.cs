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

    //public static Dictionary<int, string> ListDialog(string header, List<string> items, bool multiSelect = false, bool autoSelectIfOnlyOneItem = true, ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Blue, bool clearConsole = true)
    //{
    //    var topMargin = 5;
    //    if (items.Count == 0) return new Dictionary<int, string>();
    //    if (items.Count - topMargin > Console.WindowHeight)
    //    {
    //        clearConsole = true;
    //        var quit = false;
    //        var currentPage = 0;
    //        var pageSize = Console.WindowHeight - topMargin;
    //        while (!quit)
    //        {
    //            var pageItems = items.Skip(currentPage * pageSize).Take(pageSize).ToList();

    //            var response = ListDialogPage(header, pageItems, multiSelect, autoSelectIfOnlyOneItem, foregroundColor, backgroundColor, clearConsole, currentPage+1, pageSize);
    //            if(response.Count == 0) { return new(); }
    //            if (response.Count > 1) return response.AdjustToCurrentPage(currentPage, pageSize);
    //            if (response.First().Key < 0)
    //            {
    //                if (response.First().Value == "n")
    //                {
    //                    if (currentPage * pageSize < items.Count-pageSize) currentPage++;
    //                    continue;
    //                }
    //                if (currentPage > 0) currentPage--;
    //            }
    //            else
    //            {
    //                ToolbarService.ClearToolbar();
    //                return response.AdjustToCurrentPage(currentPage, pageSize);;
    //            }
    //        }
    //    }
    //    ToolbarService.ClearToolbar();
    //    return ListDialogPage(header, items, multiSelect, autoSelectIfOnlyOneItem, foregroundColor, backgroundColor, clearConsole);
    //}

    //private static Dictionary<int, string>  ListDialogPage(string header, List<string> items, bool multiSelect = false, bool autoSelectIfOnlyOneItem = true, ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Blue, bool clearConsole = true, int currentPage = 0)
    //{
    //    if(clearConsole) Console.Clear();
    //    WriteHeader($"{header}\n");
    //    var startRow = Console.CursorTop;
    //    var startForegroundColor = Console.ForegroundColor;
    //    var startBackgroundColor = Console.BackgroundColor;

    //    for (int index=0; index<items.Count;index++)
    //    {
    //        var item = items[index];
    //        Console.WriteLine($"{index+1}. {item}");
    //    }
    //    var quit = " ";
    //    var input = "";
    //    var selectedItems = new Dictionary<int, string>();

    //    if (items.Count == 1 && autoSelectIfOnlyOneItem)
    //    {
    //        selectedItems.Add(0, items.First());
    //        Console.WriteLine("");
    //        Console.Write(" ");
    //        ConsoleService.Service.WriteRowWithColor(3, foregroundColor, backgroundColor, $" 1. {items.First()}");
    //        return selectedItems;
    //    }

    //    var label = multiSelect ? "Enter number(s) and use enter to select them, use (a) to select all. (or just hit enter to quit)" : "Enter a number and hit enter to select. (or just hit enter to quit)";
    //    if (currentPage > 0) label += $"\nCurrent page is ({currentPage}) Enter (n) for next page (p) for previous page.";
    //    var pageEndLeft = 0;
    //    var pageEndTop = 0;
    //    while (input != quit)
    //    {
    //        if(!string.IsNullOrEmpty(label))
    //        {
    //            Console.Write($"{label}>");
    //            pageEndTop= Console.CursorTop;
    //            pageEndLeft = Console.CursorLeft;
    //        }
    //        input = ReadLineService.Service.Read().Trim();
    //        if(input == "") break;
    //        if (multiSelect && input.ToLower() == "a")
    //        {
    //            Console.Clear();
    //            Console.CursorTop = startRow;
    //            for (int i=0; i<items.Count;i++)
    //            {
    //                var item = items[i];
    //                ConsoleService.Service.WriteRowWithColor(startRow + i, foregroundColor, backgroundColor, $"{i}. {item}");
    //            }
    //            Console.CursorTop = items.Count + 1;
    //            Console.CursorLeft = 0;
    //            return items.Select((t, i) => i + 1).ToDictionary(key => key, key => items[key-1]);
    //        }
            
    //        if(currentPage > 0 && (input.Trim() == "n" || input.Trim() == "p")) return new Dictionary<int, string>(){{-1, input.Trim()}};
    //        var selectedIndex = (int.TryParse(input, out var index) ? index : 1);
    //        if(selectedIndex > items.Count || selectedIndex < 1) selectedIndex = 1;
    //        var selectedItem = new { Index = selectedIndex, Value = items[selectedIndex - 1] };
    //        var itemAdded = selectedItems.TryAdd(selectedItem.Index-1, selectedItem.Value);
            
    //        ConsoleService.Service.ClearRow(Console.CursorTop-1);
    //        var top = Console.CursorTop - 2;
    //        Console.CursorTop = Math.Clamp(top, 0, Console.LargestWindowHeight - 1);
    //        Console.CursorLeft = 0;
    //        if (currentPage > 0) startRow--;
    //        if (itemAdded)
    //        {
    //            ConsoleService.Service.WriteRowWithColor((startRow) + selectedIndex, foregroundColor, backgroundColor, $"{selectedIndex}. {selectedItem.Value}");
    //            Console.CursorTop = pageEndTop-1;
    //            Console.CursorLeft = pageEndLeft;
    //        }
    //        else
    //        {
    //            selectedItems.Remove(selectedItem.Index);
    //            ConsoleService.Service.WriteRowWithColor(startRow + selectedIndex, startForegroundColor, startBackgroundColor, $"{selectedIndex}. {selectedItem.Value}");
    //            Console.CursorTop = pageEndTop-1;
    //            Console.CursorLeft = pageEndLeft;
    //        }
    //        if (!multiSelect) break;
    //        label = "";

    //    }
    //    ToolbarService.ClearToolbar();
    //    return selectedItems;
    //}

    //private static void RenderList(List<ListDialogItem> items, int startRow, string header, string footer, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
    //{
    //    Console.CursorLeft = 0;
    //    Console.CursorTop = startRow;
    //    ConsoleService.Service.ClearRow(startRow);
        
    //    WriteHeader($"{header}\n");
    //    foreach (var item in items) Console.WriteLine();
    //    foreach (var selectedItem in items.Where(i => i.Selected))
    //    {
    //        ConsoleService.Service.WriteRowWithColor((startRow) + selectedItem.DisplayIndex, foregroundColor, backgroundColor, selectedItem.Caption);
    //    }
    //    WriteHeader($"{footer} >");
    //}

    //private static Dictionary<int, string> AdjustToCurrentPage(this Dictionary<int, string> items, int currentPage, int pageSize) => items.ToDictionary(item => item.Key + (currentPage * pageSize), item => item.Value);
    private static void WriteHeader(string text)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(text);
        Console.ForegroundColor = originalColor;
    }

    //private static void WriteLabel(string label)
    //{
    //    var originalColor = Console.ForegroundColor;
    //    Console.ForegroundColor = ConsoleColor.Green;
    //    Console.Write($"{label}");
    //    Console.ForegroundColor = originalColor;
    //}
}