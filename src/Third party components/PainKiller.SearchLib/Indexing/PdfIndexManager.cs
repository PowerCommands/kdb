using System.Text.RegularExpressions;
using PainKiller.SearchLib.DomainObjects;
using PainKiller.SearchLib.Enums;
using PdfSharpTextExtractor;

namespace PainKiller.SearchLib.Indexing;
public class PdfIndexManager(string documentFolder, string indexFile) : BaseIndexManager(documentFolder, indexFile), IIndexManager
{
    public override void IndexDocuments()
    {
        Documents.Clear();
        var pdfFiles = Directory.GetFiles(DocumentFolder, "*.pdf");
        foreach (var file in pdfFiles)
        {
            var linesWithPages = ExtractTextFromPdf(file);
            for (int i = 0; i < linesWithPages.Count; i++)
            {
                Documents.Add(new Document
                {
                    DocId = $"{file}, line {i}",
                    Type = DocumentType.Pdf,
                    Tokens = linesWithPages[i].Text.Split(new[] { ' ', '.', ',', ';', ':', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
                });
            }
        }
        SaveIndex();
        Console.WriteLine($"Indexering av PDF-filer klar! Indexerade {Documents.Count} rader.");
    }
    public DocumentType GetDocumentType() => DocumentType.Pdf;
    private List<PdfLine> ExtractTextFromPdf(string pdfPath)
    {
        List<PdfLine> lines = new();
        var currentPage = 1;
        var rawText = Extractor.PdfToText(pdfPath);
        rawText = Regex.Replace(rawText, @"<[^>]+>", ""); 
        var textLines = rawText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in textLines)
        {
            if (IsLikelyMetadata(line))
                continue;
            if (line.StartsWith("Page ") || line.ToLower().Contains("sid "))
            {
                if (int.TryParse(new string(line.Where(char.IsDigit).ToArray()), out int detectedPage))
                {
                    currentPage = detectedPage;
                    continue;
                }
            }
            lines.Add(new PdfLine { Page = currentPage, Text = line });
        }

        return lines;
    }

// Mer aggressiv filtrering av metadata
    private bool IsLikelyMetadata(string line)
    {
        string[] metadataKeywords = { "font", "color", "CMYK", "RGB", "BT", "ET", 
            "Adobe", "Illustrator", "saved", "converted",
            "XMP", "stEvt", "swatchName", "mode", "type", 
            "instanceID", "softwareAgent", "parameters"};

        return metadataKeywords.Any(keyword => line.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }
    public string GetSurroundingText(string docId)
    {
        var documentIndex = Documents.FindIndex(doc => doc.DocId == docId);
        if (documentIndex == -1) return "";

        var pageNumber = ExtractPageNumber(docId);
        string pageInfo = $"[Sid {pageNumber}]";

        // TODO: Start and end should be a dynamic setting
        var start = Math.Max(0, documentIndex - 5);
        var end = Math.Min(Documents.Count - 1, documentIndex + 5);

        var surroundingLines = Documents[start..(end + 1)].Select(doc => string.Join(" ", doc.Tokens));

        return $"{pageInfo}\n" + string.Join("\n", surroundingLines);
    }

    private int ExtractPageNumber(string docId)
    {
        var match = Regex.Match(docId, @"\[Sid (\d+)\]");
        return match.Success ? int.Parse(match.Groups[1].Value) : 1; // Standard: Sida 1 om inget sidnummer hittas
    }

    private class PdfLine
    {
        public int Page { get; set; }
        public string Text { get; init; } = "";
    }
}
