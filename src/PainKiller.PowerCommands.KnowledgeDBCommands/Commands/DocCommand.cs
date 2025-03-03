using PainKiller.PowerCommands.Configuration.DomainObjects;
using PainKiller.PowerCommands.Core.Commands;
using PainKiller.SearchLib.Indexing;
using PainKiller.SearchLib.Managers;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Commands;
[PowerCommandTest(         tests: " ")]
[PowerCommandDesign( description: "Add or search document",
                         options: "add|convert",
                         example: "doc <search>")]
public class DocCommand(string identifier, PowerCommandsConfiguration configuration) : CdCommand(identifier, configuration)
{
    public override RunResult Run()
    {
        if (HasOption("add")) return AddFile();
        if (HasOption("convert")) return ConvertPdfToText();

        var folderPath = Path.Combine(ConfigurationGlobals.ApplicationDataFolder, "Docs");
        var textManager = new TextIndexManager(folderPath, "text_index.json");
        var pdfManager = new PdfIndexManager(folderPath, "pdf_index.json");

        List<IIndexManager> indexManagers = [textManager, pdfManager];
        var indexManager = new IndexManager(folderPath);
        indexManager.IndexDocuments();

        var searchEngine = new BM25SearchEngine();
        searchEngine.LoadIndex(indexManager.GetCombinedIndex());
        ConsoleService.Service.Clear();

        var query = string.Join(" ", Input.Arguments);
        var results = searchEngine.Search(query, 5, indexManagers);

        var selectedIndex = ListService.ListDialog($"\nB�sta matchningar: {query}", results.Select(r => $"{r.DocId} page: {r.PageNumber} relevans: {r.Score}").ToList());
        if (selectedIndex.Count <= 0) return Ok();

        var selected = results[selectedIndex.Keys.First()];
        WriteHeadLine($"\n{selected.DocId} ");
        WriteLine(selected.Content);

        return Ok();

    }
    private RunResult AddFile()
    {
        var docDirectory = Path.Combine(ConfigurationGlobals.ApplicationDataFolder, "Docs");
        var txtFiles = Directory.GetFiles(Environment.CurrentDirectory, "*.txt");
        var pdfFiles = Directory.GetFiles(Environment.CurrentDirectory, "*.pdf");

        var allFiles = txtFiles.Concat(pdfFiles).ToArray();

        var selectedFile = DialogService.SelectFileDialog(allFiles);
        var fileInfo = new FileInfo(selectedFile);
        var targetFile = Path.Combine(docDirectory, fileInfo.Name);

        File.Copy(selectedFile, targetFile);
        WriteSeparatorLine();
        WriteSuccessLine($"File [{selectedFile}] copied to [{targetFile}] OK!");

        return Ok();
    }
    private RunResult ConvertPdfToText()
    {
        var folderPath = Path.Combine(ConfigurationGlobals.ApplicationDataFolder, "Docs\\pdf");
        PdfToTextManager.ConvertPdfToText(folderPath);
        return Ok();
    }
}