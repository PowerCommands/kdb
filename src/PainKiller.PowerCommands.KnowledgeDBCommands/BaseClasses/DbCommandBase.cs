namespace PainKiller.PowerCommands.KnowledgeDBCommands.BaseClasses;

public class DbCommandBase : CommandBase<PowerCommandsConfiguration>
{
    protected readonly DbManager DBManager;
    public DbCommandBase(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration)  => DBManager = new(Configuration.DatabaseFileName);
    protected IEnumerable<KnowledgeItem> GetAllItems() => DBManager.GetAll().ToList();
}