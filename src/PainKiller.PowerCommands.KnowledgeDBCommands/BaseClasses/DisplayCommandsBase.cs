using PainKiller.PowerCommands.Core.BaseClasses;
using PainKiller.PowerCommands.Shared.DomainObjects.Core;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.BaseClasses;

public abstract class DisplayCommandsBase : CommandWithToolbarBase<PowerCommandsConfiguration>
{
    protected static List<KnowledgeItem> Items = new();
    protected static KnowledgeItem? SelectedItem;
    protected readonly IStorageService<KnowledgeDatabase> Storage;

    protected DisplayCommandsBase(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) => Storage = StorageService<KnowledgeDatabase>.Service;

    public override RunResult Run()
    {
        return ContinueWith("");
    }
    protected void Open(KnowledgeItem match)
    {
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
}