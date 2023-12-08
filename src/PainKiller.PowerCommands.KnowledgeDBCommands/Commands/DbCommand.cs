using PainKiller.PowerCommands.Configuration.DomainObjects;
using PainKiller.PowerCommands.KnowledgeDBCommands.BaseClasses;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Commands;

[PowerCommandsToolbar("[Press enter to view stats]|[--edit](edit file)")]
[PowerCommandTest(         tests: " ")]
[PowerCommandDesign( description: "Open the database file with your configured editor or view stats.",
                         options: "edit|duplicates",
                         example: "//Show stats|db|//Edit database file with your configured editor|db --edit|//Show duplicates|db --duplicates")]
public class DbCommand : DisplayCommandsBase
{
    public DbCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override RunResult Run()
    {
        if (HasOption("edit"))
        {
            try
            {
                ShellService.Service.Execute(Configuration.CodeEditor, arguments: Configuration.DatabaseFileName, ConfigurationGlobals.ApplicationDataFolder, WriteLine, fileExtension: "");
            }
            catch (Exception) { return BadParameterError("Your editor must be included in Path environment variables"); }
            return Ok();
        }
        Console.Clear();
        if (HasOption("duplicates"))
        {
            ShowDuplicates();
            return Ok();
        }
        var items = GetAllItems().ToList();
        var stats = DBManager.GetStats();
        WriteHeadLine(" Stats");
        WriteCodeExample("Total count",$"{stats.Count}");
        WriteCodeExample("Never opened count", $"{stats.NeverOpenedCount}");
        WriteCodeExample("\n File size", $"{stats.DisplayFileSize}");
        WriteCodeExample("Last updated", $"{stats.DisplayLastUpdated}");
        
        var quit = false;
        var pageIndex = 0;
        var pageSize = 20;
        while (!quit)
        {
            Items = items.OrderBy(i => i.Updated).ThenBy(i => i.Created).Skip(pageIndex*pageSize).Take(pageSize).ToList();
            var itemSelected = ShowResult("\n Oldest posts in database", clearConsole: false);
            if (itemSelected) return Ok();

            pageIndex++;
            quit = !DialogService.YesNoDialog($"Do want to list next {pageSize} items?");
            Console.Clear();
        }
        return Ok();
    }
    public void ShowDuplicates()
    {
        Items = DBManager.GetItemsWithDuplicateUri().Where(i => i.Tags.StartsWith("#")).ToList();
        ShowResult("Duplicates in database");
    }
}