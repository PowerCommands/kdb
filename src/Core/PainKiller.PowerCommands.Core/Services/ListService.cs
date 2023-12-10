namespace PainKiller.PowerCommands.Core.Services;

public static class ListService
{
    private const int topMargin = 5;
    private static List<ListDialogItem> ToListDialogItems(this List<string> items)
    {
        var pageSize = Console.WindowHeight - topMargin;
        var retVal = new List<ListDialogItem>();
        var index = 0;
        foreach (var item in items)
        {
            var dialogItem = new ListDialogItem{ItemIndex = index};
            var currentPage = index / pageSize;
            dialogItem.DisplayIndex = index+1;
            dialogItem.Caption = $"{dialogItem.DisplayIndex}. {item}";
            dialogItem.RowIndex = dialogItem.DisplayIndex - (currentPage * pageSize);
            retVal.Add(dialogItem);
            index++;
        }
        return retVal;
    }

    private static Dictionary<int, string> ToDictionary(this List<ListDialogItem> items)
    {
        var dictionary = new Dictionary<int, string>();
        foreach (var item in items) dictionary[item.ItemIndex] = item.Caption;
        return dictionary;
    }
    public static Dictionary<int, string> ListDialog(string header, List<string> items, bool multiSelect = false, bool autoSelectIfOnlyOneItem = true, ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Blue, bool clearConsole = true)
    {
        if (items.Count == 0) return new Dictionary<int, string>();
        
        var listItems = items.ToListDialogItems();
        if (listItems.Count - topMargin > Console.WindowHeight)
        {
            clearConsole = true;
            var quit = false;
            var currentPage = 0;
            var pageSize = Console.WindowHeight - topMargin;
            while (!quit)
            {
                var pageItems = listItems.Skip(currentPage * pageSize).Take(pageSize).ToList();
                var response = ListDialogPage(header, pageItems, multiSelect, autoSelectIfOnlyOneItem, foregroundColor, backgroundColor, clearConsole, currentPage);
                if(response.Count == 0) { return new(); }

                if (response.Count > 1) return response.ToDictionary();
                if (response.First().ItemIndex < 0)
                {
                    if (response.First().Caption == "n")
                    {
                        if (currentPage * pageSize < items.Count - pageSize) currentPage++;
                        continue;
                    }
                    if (currentPage > 0) currentPage--;
                }
                else
                {
                    ToolbarService.ClearToolbar();
                    return response.ToDictionary(); 
                }
            }
        }
        ToolbarService.ClearToolbar();
        return ListDialogPage(header, listItems, multiSelect, autoSelectIfOnlyOneItem, foregroundColor, backgroundColor, clearConsole).ToDictionary();
    }

    private static List<ListDialogItem> ListDialogPage(string header, List<ListDialogItem> items, bool multiSelect = false, bool autoSelectIfOnlyOneItem = true, ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Blue, bool clearConsole = true, int currentPage = 0)
    {
        if (clearConsole) Console.Clear();
        WriteLabel($"{header}\n");
        var startRow = Console.CursorTop;
        var selectedItems = new List<ListDialogItem>();
        
        var footer = multiSelect ? "Enter number(s) and use enter to select them. (blank to quit)" : "Enter a number and hit enter to select. (blank to quit)";
        var listCaption =  (currentPage > 0) ? $"Page ({currentPage}) Enter (n) for next page (p) for previous, (a) select all." : "Use (a) to select all";
        RenderList(items, startRow, listCaption, footer, foregroundColor, backgroundColor);

        if (items.Count == 1 && autoSelectIfOnlyOneItem)
        {
            RenderList(items, startRow, "Autoselected", "", foregroundColor, backgroundColor);
            selectedItems.Add(items.First());
            return selectedItems;
        }

        var quit = " ";
        var input = "";

        while (input != quit)
        {
            input = $"{Console.ReadLine()}".Trim().ToLower();
            if(input == "") break;
            
            if (multiSelect && input == "a")
            {
                RenderList(items.Select(i => new ListDialogItem{Caption = i.Caption,DisplayIndex = i.DisplayIndex,ItemIndex = i.ItemIndex,RowIndex = i.RowIndex,Selected = true}).ToList(), startRow, listCaption, footer, foregroundColor, backgroundColor);
                return items;
            }

            if ((input == "n" || input == "p")) return new List<ListDialogItem> { new() { ItemIndex = -1, Caption = input } };

            var selectedIndex = (int.TryParse(input, out var index) ? index : 1);
            var selectedItem = items.First(i => i.DisplayIndex == selectedIndex);
            selectedItems.Remove(selectedItem);
            selectedItem.Selected = !selectedItem.Selected;
            selectedItems.Add(selectedItem);
            RenderList(items, startRow, listCaption, footer, foregroundColor, backgroundColor);

        }
        return selectedItems;
    }

    private static void RenderList(List<ListDialogItem> items, int startRow, string header, string footer, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
    {
        Console.CursorLeft = 0;
        Console.CursorTop = startRow;
        ConsoleService.Service.ClearRow(startRow);
        
        WriteHeader($"{header}\n");
        foreach (var item in items) Console.WriteLine(item.Caption);
        foreach (var selectedItem in items.Where(i => i.Selected))
        {
            ConsoleService.Service.WriteRowWithColor((startRow) + selectedItem.RowIndex, foregroundColor, backgroundColor, selectedItem.Caption);
        }
        WriteHeader($"{footer} >");
    }

    private static void WriteHeader(string text)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(text);
        Console.ForegroundColor = originalColor;
    }

    private static void WriteLabel(string label)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"{label}");
        Console.ForegroundColor = originalColor;
    }
}