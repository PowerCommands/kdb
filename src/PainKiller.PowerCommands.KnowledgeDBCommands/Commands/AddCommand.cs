using PainKiller.PowerCommands.KnowledgeDBCommands.BaseClasses;
using PainKiller.PowerCommands.KnowledgeDBCommands.Enums;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Commands;

[PowerCommandDesign(  description: "Add a new knowledge item or crawl a directory to find items (subdirectories and files)",
                        arguments: "<type>",
                          options: "!name|!tags|!directory",
                      suggestions: "url|onenote|path|file",
                          example: "//Add url|add url \"https://wiki/wikis\" --name \"WikiStart\" --tags wiki,start|//Add path|add path \"C:\\temp\\project\" --name \"project directory\" --tags document")]

public class AddCommand(string identifier, PowerCommandsConfiguration configuration) : DisplayCommandsBase(identifier, configuration)
{
    public override RunResult Run()
    {
        if (HasOption("directory")) return AddDirectory(GetOptionValue("directory"));

        var item = new KnowledgeItem
        {
            Uri = Input.Raw.Contains("\"") ? Input.SingleQuote : string.Join(" ", Input.Arguments),
            Name = DialogService.QuestionAnswerDialog("Enter the name of the new item","Name:"),
            SourceType = ToolbarService.NavigateToolbar<ItemSourceType>().ToString(),
            Tags = DialogService.QuestionAnswerDialog("Enter the tags to be used to find this item, separate them with ,","Tags:")
        };
        Details(item);
        
        if (!DialogService.YesNoDialog("Is this information correct?")) return Ok();
        
        if (DBManager.Exists(item))
        {
            WriteLine($"Warning, there is already a item with the same name and source stored in DB");
            return Ok();
        }
        DBManager.Create(item);

        WriteLine($"Item {item.Name} has successfully been added.");
        return Ok();
    }
    public RunResult AddDirectory(string tag)
    {
        var path = Environment.CurrentDirectory;
        var skipFiles = !DialogService.YesNoDialog("Do you want to include the files?");

        var prospects = DirectoryIteratorManager.GetItemsFromDirectory(DBManager.GetAll().ToList(), Configuration.FileTypesFileName, path, true, tag: tag).Where(i => i.Exists == false && (!i.Prospect.Tags.Contains(DirectoryIteratorManager.FileTag) || !skipFiles)).ToList();
        var table =  prospects.Select((i, idx) => new{Index = idx, i.Prospect.Name, Source = i.Prospect.SourceType, i.Prospect.Tags}).ToList();
        ConsoleTableService.RenderTable(table, this);
        var addQuery = DialogService.YesNoDialog("Do you want to add all this items?");
        if (addQuery)
        {
            
            var fileName = DBManager.Backup();
            WriteLine($"File was first backed up to {fileName}");

            var items = prospects.Select(p => p.Prospect).ToList();
            DBManager.Create(items);
            WriteSuccessLine($"{items.Count} items was added to the knowledge database.");
        }
        return Ok();
    }
}