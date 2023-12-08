using System.Text.Json;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Managers;

public static class DirectoryIteratorManager
{
    public const string FileTag = "#FILE";
    public const string DirTag = "#DIR";

    private static KnowledgeItemFileTypes? _fileTypes;
    public static IEnumerable<KnowledgeItemProspect> GetItemsFromDirectory(List<KnowledgeItem> dbItems,string fileTypesFileName, string directoryPath, bool includeSubDirectories, string tag)
    {
        var jsonString = File.ReadAllText(fileTypesFileName);
        _fileTypes = JsonSerializer.Deserialize<KnowledgeItemFileTypes>(jsonString) ?? new KnowledgeItemFileTypes();

        var retVal = new List<KnowledgeItemProspect>();
        var rootDirectory = new DirectoryInfo(directoryPath);
        var items = GetItems(directoryPath, includeSubDirectories,  rootDirectory.Name, tag);
        retVal.AddRange(items.Select(i => new KnowledgeItemProspect(i, dbItems.Any(db => db.Uri == i.Uri))));
        return retVal;
    }
    private static IEnumerable<KnowledgeItem> GetItems(string directoryPath, bool includeSubDirectories, string parentDirectoryName, string tag)
    {
        var retVal = new List<KnowledgeItem>();
        var rootDirectory = new DirectoryInfo(directoryPath);
        var uniqueTag = $"#{DateTime.Now.ToShortDateString()}";
        retVal.Add(new KnowledgeItem{Name = rootDirectory.Name, SourceType = "path", Uri = rootDirectory.FullName, Tags = $"{DirTag},{parentDirectoryName},{tag},{uniqueTag}"});
        if (includeSubDirectories) foreach (var directoryInfo in rootDirectory.GetDirectories()) retVal.AddRange(GetItems(directoryInfo.FullName, includeSubDirectories: true, rootDirectory.Name, tag));
        var notAllowedFileExtensions = new[] { ".exe", ".bat", ".vbs", ".cmd", ".com", ".cpl", ".dll", ".js", ".jse", ".msc", ".msh", ".msh1", ".msh2", ".mshxml", ".msh1xml", ".msh2xml", ".pif", ".ps1", ".ps1xml", ".ps2", ".ps2xml", ".psc1", ".psc2", ".reg", ".scf", ".scr", ".sct", ".shb", ".sys", ".vb", ".vbe", ".ws", ".wsf", ".wsh" };
        
        foreach (var fileInfo in rootDirectory.GetFiles())
        {
            if(notAllowedFileExtensions.Any(f => f == fileInfo.Extension.ToLower())) continue;
            var fileType = _fileTypes?.FileTypes.FirstOrDefault(f => string.Equals(f.Extension, fileInfo.Extension, StringComparison.OrdinalIgnoreCase));
            if(fileType == null) continue;
            var uri = fileType.OpenDirectory ? rootDirectory.FullName : fileInfo.FullName;
            retVal.Add(new KnowledgeItem{Name = fileInfo.Name, SourceType = "file", Uri = uri, Tags = $"{rootDirectory.Name},{FileTag},#{fileType.Name},{tag},{uniqueTag}"});
        }
        return retVal;
    }
}