using System.Collections.Generic;
using System.Linq;
using PainKiller.SearchLib.DomainObjects;
using PainKiller.SearchLib.Indexing;

namespace PainKiller.SearchLib.Managers;
public class IndexManager(string documentFolder)
{
    private readonly TextIndexManager _textManager = new(documentFolder, "text_index.json");
    private readonly PdfIndexManager _pdfManager = new(documentFolder, "pdf_index.json");
    public void IndexDocuments()
    {
        _textManager.IndexDocuments();
        _pdfManager.IndexDocuments();
    }
    public List<Document> GetCombinedIndex() => _textManager.GetDocuments().Concat(_pdfManager.GetDocuments()).ToList();
}
