namespace PainKiller.PowerCommands.KnowledgeDBCommands.DomainObjects;

public class KnowledgeDbStats
{
    public int Count { get; set; }
    public int NeverOpenedCount { get; set; }
    public string DisplayLastUpdated { get; set; } = "";
    public string DisplayFileSize { get; set; } = "";

}