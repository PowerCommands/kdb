using PainKiller.PowerCommands.KnowledgeDBCommands.BaseClasses;
using PainKiller.PowerCommands.ReadLine;
using System.Text.RegularExpressions;
using PainKiller.PowerCommands.Core.Commands;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Commands;

[PowerCommandDesign(  description: "Add a new knowledge item or crawl a directory to find items (subdirectories and files)",
                        arguments: "<type>",
                          options: "!name|!directory",
                      suggestions: "url|onenote|path|file",
                          example: "//Add url|add url \"https://wiki/wikis\" --name \"WikiStart\" --tags wiki,start|//Add path|add path \"C:\\temp\\project\" --name \"project directory\" --tags document")]

public class AddCommand : DisplayCommandsBase
{
    public AddCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) => ReadLineService.CmdLineTextChanged += ReadLineService_CmdLineTextChanged;
    private void ReadLineService_CmdLineTextChanged(object? sender, ReadLine.Events.CmdLineTextChangedArgs e)
    {
        if(!e.CmdText.StartsWith("add")) return;
        var args = e.CmdText.Split(' ');
        var labels = new []{""};
        switch (args.Length)
        {
            case 1:
                 if (!string.IsNullOrEmpty(args[0])) labels = new []{""};
                break;
            case 2:
                labels = new[] { "<type> (press [tab])","(url,onenote,path,file)"};
                break;
            case 3:
                labels = new[] { args[1],"\"<value>\""};
                break;
            case 4:
                labels = new []{"[Option] (press - then [tab])","--name"};
                break;
            case 5:
                labels = new []{"[Option]","--name","\"<value>\""};
                break;
            case 6:
                labels = Regex.Matches(string.Join(' ', args),Regex.Escape("\"")).Count == 4 ? new []{"[Option] (press - then [tab])","--tags"} : new []{"[Option]","--name","\"<value>\""};
                break;
            default:
                if(string.Join(' ', args).Contains("--tags")) labels = new []{"[Option]","--tags","<comma separated values>"};
                break;
        }
        ToolbarService.DrawToolbar(labels);
    }
    public override RunResult Run()
    {
        if (HasOption("directory")) return AddDirectory(GetOptionValue("directory"));

        var item = new KnowledgeItem(Input);
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
        var path = CdCommand.WorkingDirectory;
        var skipFiles = !DialogService.YesNoDialog("Do you want to include the files?");

        var prospects = DirectoryIteratorManager.GetItemsFromDirectory(DBManager.GetAll().ToList(), Configuration.FileTypesFileName, path, true, tag: tag).Where(i => i.Exists == false && (!i.Prospect.Tags.Contains(DirectoryIteratorManager.FileTag) || !skipFiles)).ToList();
        var table =  prospects.Select((i, idx) => new{Index = idx++, Name = i.Prospect.Name, Source = i.Prospect.SourceType, Tags= i.Prospect.Tags}).ToList();
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