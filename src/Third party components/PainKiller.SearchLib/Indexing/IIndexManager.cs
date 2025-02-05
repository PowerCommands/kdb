using PainKiller.SearchLib.DomainObjects;
using PainKiller.SearchLib.Enums;

namespace PainKiller.SearchLib.Indexing;
public interface IIndexManager
{
    void IndexDocuments();
    string GetSurroundingText(string docId);
    List<Document> GetDocuments();
    string SaveIndex();
    void LoadIndex();
    DocumentType GetDocumentType();
}
