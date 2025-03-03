using PainKiller.PowerCommands.KnowledgeDBCommands.Extensions;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.BaseClasses;

public abstract class DisplayCommandsBase(string identifier, PowerCommandsConfiguration configuration) : DbCommandBase(identifier, configuration)
{
    protected const byte MaxAutoOpenItems = 10;
    protected static List<KnowledgeItem> Items = new();
    protected static List<KnowledgeItem> SelectedItems = new();
    protected static bool MultiSelect = false;

    public override RunResult Run()
    {
        return ContinueWith("");
    }
    protected void Open(KnowledgeItem match)
    {
        DBManager.RefreshUpdated(match);

        IShellExecuteManager? shellExecuteManager = null;
        switch (match.SourceType.ToLower())
        {
            case "onenote":
                shellExecuteManager = new OneNoteManager();
                break;
            case "path":
            case "directory":
                shellExecuteManager = new OpenFolderManager();
                break;
            case "url":
            case "file":
                shellExecuteManager = new BrowserManager();
                break;
            default:
                shellExecuteManager = new BrowserManager();
                WriteLine($"The source type {match.SourceType} is not supported, default open method will be used, it may not work properly.");
                break;
        }
        WriteHeadLine($"Opening [{match.Uri}] with [{shellExecuteManager?.GetType().Name}]");
        shellExecuteManager?.Run(Configuration.Shell, match.Uri);
    }
    protected void Details(KnowledgeItem item)
    {
        WriteHeadLine($"Details");
        WriteLine($"{item}");
    }
    protected bool ShowResult(string headLine)
    {
        var selected = ListService.ListDialog($"{headLine}\nSearch phrase(s): {Input.Raw.Replace("find ","")}", Items.Select(i => $"{i.Name} {i.SourceType} {i.Uri.Display(Configuration.DisplayUrlMaxLength)} {i.Tags.Display(Configuration.DisplayTagsMaxLength)}").ToList(), multiSelect: MultiSelect);
        if (selected.Count == 0) return false;
        
        SelectedItems.Clear();
        foreach (var key in selected.Keys) SelectedItems.Add(Items[key]);
        
        ToolbarService.DrawToolbar([$"[Action] ->", "open (CTRL+O)", "edit", "delete", "tags"]);
        return true;
    }
    protected void ShowSelectedItems()
    {
        WriteHeadLine("Current selected items");
        ConsoleTableService.RenderTable(SelectedItems.Select(i => new{ i.Name, Source = i.SourceType, i.Created, i.Tags}), this);
    } 
}