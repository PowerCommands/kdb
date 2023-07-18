namespace PainKiller.PowerCommands.KnowledgeDBCommands.Extensions;
public static class KdbExtensions
{
    public static KnowledgeItem ToItem(this string editValues)
    {
        var values = editValues.Split('|');
        var retVal = new KnowledgeItem{Name = ExtractPropertyValue(values, nameof(KnowledgeItem.Name).ToLower()), Uri = ExtractPropertyValue(values, nameof(KnowledgeItem.Uri).ToLower()), SourceType = ExtractPropertyValue(values, nameof(KnowledgeItem.SourceType).ToLower()), Tags = ExtractPropertyValue(values, nameof(KnowledgeItem.Tags).ToLower())};
        return retVal;
    }
    private static string ExtractPropertyValue(string[] values, string name) => (values.FirstOrDefault(v => v.StartsWith($"{name}=")) ?? "").Replace($"{name}=","");
}