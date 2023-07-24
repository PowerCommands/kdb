using System.Text.RegularExpressions;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Extensions;
public static class KdbExtensions
{
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
    public static DateTime ExtractDateTimeFromFileName(this string fileName)
    {
        var pattern = @"\d{14}"; // Matches 14 digits (yyyymmddhhmmss).
        var match = Regex.Match(fileName, pattern);
        
        if (!match.Success) return DateTime.MinValue;
        
        var dateTimeString = match.Value;
        if (DateTime.TryParseExact(dateTimeString, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime result)) return result;
        return DateTime.MinValue;
    }
}