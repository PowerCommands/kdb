namespace PainKiller.PowerCommands.KnowledgeDBCommands.DomainObjects;

public class KnowledgeItemProspect
{
    public KnowledgeItemProspect(KnowledgeItem prospect, bool exists)
    {
        Prospect = prospect;
        Exists = exists;
    }
    public KnowledgeItem Prospect { get;}
    public bool Exists { get; }
}