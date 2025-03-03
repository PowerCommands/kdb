using System;
using System.IO;
using PainKiller.SearchLib.DomainObjects;
using PainKiller.SearchLib.Enums;

namespace PainKiller.SearchLib.Indexing;
public class TextIndexManager(string documentFolder, string indexFile) : BaseIndexManager(documentFolder, indexFile), IIndexManager
{
    public override void IndexDocuments()
    {
        Documents.Clear();
        var txtFiles = Directory.GetFiles(DocumentFolder, "*.txt");

        foreach (var file in txtFiles)
        {
            var lines = File.ReadAllLines(file);
            for (int i = 0; i < lines.Length; i++)
            {
                var currentPage = GetPageNumber(lines, i);
                Documents.Add(new Document
                {
                    DocId = $"{file}, line {i}",
                    PageNumber = currentPage,
                    Type = DocumentType.Text,
                    Tokens = lines[i].ToLower().Split(new[] { ' ', '.', ',', ';', ':', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
                });
            }
        }
        SaveIndex();
    }
    public DocumentType GetDocumentType() => DocumentType.Text;
    public string GetSurroundingText(string docId)
    {
        var parts = docId.Split(", line ");
        if (parts.Length < 2) return "";

        var filePath = parts[0];
        if (!File.Exists(filePath)) return "";

        var lineIndex = int.Parse(parts[1]);
        var allLines = File.ReadAllLines(filePath);

        var start = Math.Max(0, lineIndex - 5);
        var end = Math.Min(allLines.Length - 1, lineIndex + 5);

        return string.Join("\n", allLines[start..(end + 1)]);
    }

    public string GetPageNumber(string[] lines, int currentLineIndex)
    {
        for (int i = currentLineIndex; i > -1; i--)
        {
            var line = lines[i];
            if (line.StartsWith("PageCounter"))
            {
                return line.Replace($"PageCounter ", "");
            }
        }
        return "1";
    }
    
}
