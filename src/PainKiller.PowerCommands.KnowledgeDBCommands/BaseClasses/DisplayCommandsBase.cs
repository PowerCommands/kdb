namespace PainKiller.PowerCommands.KnowledgeDBCommands.BaseClasses;

public abstract class DisplayCommandsBase : CommandWithToolbarBase<PowerCommandsConfiguration>
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
    protected KnowledgeDatabase GetDb() => StorageService<KnowledgeDatabase>.Service.GetObject(Configuration.DatabaseFileName);
    protected void Save(KnowledgeDatabase db) => StorageService<KnowledgeDatabase>.Service.StoreObject(db, Configuration.DatabaseFileName);
}