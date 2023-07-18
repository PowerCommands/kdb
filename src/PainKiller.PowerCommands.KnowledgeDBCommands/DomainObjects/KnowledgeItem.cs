using System.Text.Json;
using System.Text.Json.Serialization;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.DomainObjects;

public class KnowledgeItem
{
    public KnowledgeItem(){}

    public KnowledgeItem(ICommandLineInput input)
    {
        ItemID = Guid.NewGuid();
        Name = input.GetOptionValue(nameof(Name).ToLower());
        SourceType = input.SingleArgument;
        Uri = input.SingleQuote;
        Tags = input.GetOptionValue(nameof(Tags).ToLower());
    }
    public Guid? ItemID { get; set; }
    public string Name { get; set; } = "";
    public string SourceType { get; set; } = "";
    public string Uri { get; set; } = "";
    public DateTime Created { get; set; } = DateTime.Now;
    public string Tags { get; set; } = "";
    public override string ToString()
    {
        var options = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true };
        var jsonString = JsonSerializer.Serialize(this, options);
        return jsonString;
    }
}