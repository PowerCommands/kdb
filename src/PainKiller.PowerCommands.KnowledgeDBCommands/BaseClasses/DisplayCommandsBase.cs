using PainKiller.PowerCommands.Configuration.DomainObjects;
using PainKiller.PowerCommands.KnowledgeDBCommands.Extensions;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.BaseClasses;

public abstract class DisplayCommandsBase : DbCommandBase
{
    
    protected static List<KnowledgeItem> Items = new();
    protected static KnowledgeItem? SelectedItem;
    protected DisplayCommandsBase(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration){}

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
        Write($"\n{ConfigurationGlobals.Prompt}");
    }
    protected void Details(KnowledgeItem item)
    {
        WriteHeadLine($"Details");
        WriteLine($"{item}");
    }
    protected bool ShowResult(string headLine, bool clearConsole = true)
    {
        if(clearConsole) Console.Clear();
        WriteHeadLine($"{headLine}\n");
        var selected = DialogService.ListDialog("Enter a valid number to select item, or just hit enter to cancel.", Items.Select(i => $"{i.Name} {i.SourceType} {i.Uri.Display(Configuration.DisplayUrlMaxLength)} {i.Tags.Display(Configuration.DisplayTagsMaxLength)}").ToList(), clearConsole: false);
        if (selected.Count == 0) return false;
        var selectedIndex = selected.First().Key;
        SelectedItem = Items[selectedIndex];

        ToolbarService.DrawToolbar(new[] { $"[Action] ->", "open (CTRL+O)", "edit", "delete", "tags" });
        return true;
    }
}