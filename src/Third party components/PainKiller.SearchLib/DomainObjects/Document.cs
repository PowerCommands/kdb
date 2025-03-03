using PainKiller.SearchLib.Enums;
using System.Text.Json.Serialization;

namespace PainKiller.SearchLib.DomainObjects;
public class Document
{
    public string DocId { get; set; } = "";
    public string PageNumber { get; set; } = "Unknown";

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DocumentType Type { get; set; }
    public string[] Tokens { get; set; } = [];
}
