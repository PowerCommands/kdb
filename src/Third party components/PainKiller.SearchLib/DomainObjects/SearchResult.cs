namespace PainKiller.SearchLib.DomainObjects;
public class SearchResult
{
    public string DocId { get; set; } = "";
    public double Score { get; set; }
    public string Content { get; set; } = "";
    public string PageNumber { get; set; } = "Unknown";
}
