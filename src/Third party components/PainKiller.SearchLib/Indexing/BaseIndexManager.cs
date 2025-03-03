using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using PainKiller.SearchLib.DomainObjects;

namespace PainKiller.SearchLib.Indexing;

public abstract class BaseIndexManager(string documentFolder, string indexFile)
{
    protected readonly string DocumentFolder = documentFolder;
    protected readonly string IndexFilePath = indexFile;
    protected List<Document> Documents = [];

    public abstract void IndexDocuments();
    public List<Document> GetDocuments()
    {
        if(Documents.Count == 0) LoadIndex();
        return Documents;
    }
    public string SaveIndex()
    {
        var json = JsonSerializer.Serialize(Documents, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Path.Combine(AppContext.BaseDirectory, IndexFilePath), json);
        return IndexFilePath;
    }
    public void LoadIndex()
    {
        if (!File.Exists(IndexFilePath)) return;
        var json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, IndexFilePath));
        Documents = JsonSerializer.Deserialize<List<Document>>(json) ?? new List<Document>();
    }
}
