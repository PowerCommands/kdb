using PainKiller.SearchLib.Enums;
using System.Text.Json.Serialization;

namespace PainKiller.SearchLib.DomainObjects;
public class Document
{
    public string DocId { get; set; } = "";

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DocumentType Type { get; set; }
    public string[] Tokens { get; set; } = [];
}
