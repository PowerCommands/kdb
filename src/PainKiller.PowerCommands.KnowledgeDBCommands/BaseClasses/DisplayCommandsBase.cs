using PainKiller.PowerCommands.KnowledgeDBCommands.Extensions;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.BaseClasses;

public abstract class DisplayCommandsBase(string identifier, PowerCommandsConfiguration configuration) : DbCommandBase(identifier, configuration)
{
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
        switch (match.SourceType)
        {
            case "onenote":
                shellExecuteManager = new OneNoteManager();
                break;
            case "path":
                shellExecuteManager = new OpenFolderManager();
                break;
            case "url":
            case "file":
                shellExecuteManager = new BrowserManager();
                break;
            default:
                WriteLine($"The source type {match.SourceType} is not supported, only onenote path or url is valid, you can change that with --edit --source option on the object, see examples");
                break;
        }
        WriteHeadLine($"Opening [{match.Uri}] with [{shellExecuteManager?.GetType().Name}]");
        shellExecuteManager?.Run(Configuration.ShellConfiguration, match.Uri);
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
        
        ToolbarService.DrawToolbar(new[] { $"[Action] ->", "open (CTRL+O)", "edit", "delete", "tags" });
        return true;
    }
    protected void ShowSelectedItems()
    {
        WriteHeadLine("Current selected items");
        ConsoleTableService.RenderTable(SelectedItems.Select(i => new{ i.Name, Source = i.SourceType, i.Created, i.Tags}), this);
    } 
}