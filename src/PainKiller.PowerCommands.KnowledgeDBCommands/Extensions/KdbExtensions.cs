using PainKiller.PowerCommands.KnowledgeDBCommands.Enums;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Extensions;
public static class KdbExtensions
{
    public static ItemSourceType ToItemSourceType(this KnowledgeItem item)
    {
        return item.SourceType switch
        {
            "onenote" => ItemSourceType.OneNote,
            "path" => ItemSourceType.Directory,
            "url" => ItemSourceType.Url,
            "file" => ItemSourceType.File,
            _ => ItemSourceType.Unknown
        };
    }
    public static KnowledgeItem ToItem(this string editValues)
    {
        var values = editValues.Split('|');
        var retVal = new KnowledgeItem{Name = ExtractPropertyValue(values, nameof(KnowledgeItem.Name).ToLower()), Uri = ExtractPropertyValue(values, nameof(KnowledgeItem.Uri).ToLower()), SourceType = ExtractPropertyValue(values, nameof(KnowledgeItem.SourceType).ToLower()), Tags = ExtractPropertyValue(values, nameof(KnowledgeItem.Tags).ToLower())};
        return retVal;
    }
    public static string Display(this string value, int maxLength)
    {
        if(value.Length <= maxLength) return value;
        return value.Substring(0, maxLength).PadRight(maxLength+3,'.');
    }
    private static string ExtractPropertyValue(string[] values, string name) => (values.FirstOrDefault(v => v.StartsWith($"{name}=")) ?? "").Replace($"{name}=","");
}