using PainKiller.PowerCommands.KnowledgeDBCommands.BaseClasses;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Commands;

[PowerCommandsToolbar("Press -> [Enter]")]
[PowerCommandDesign(  description: "Delete the current selected item",
                        arguments: "",
                          options: "!tag",
                          example: "//First search and select one item|delete")]
public class DeleteCommand : DisplayCommandsBase
{
    public DeleteCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }
    public override RunResult Run()
    {
        if (HasOption("tag")) return DeleteTag(GetOptionValue("tag"));
        Console.Clear();
        ShowSelectedItems();
        foreach (var item in SelectedItems)
        {
            var cancel = Delete(item);
            if(cancel) break;
        }
        return Ok();
    }
    private bool Delete(KnowledgeItem item)
    {
        Details(item);

        var quit = !DialogService.YesNoDialog($"Are you sure you want to delete the item?");
        if (quit) return true;
        
        DBManager.Delete(item.ItemID.GetValueOrDefault());
        WriteLine($"Item {item.ItemID} {item.Name} removed.");
        return false;
    }
    private RunResult DeleteTag(string tag)
    {
        var items = DBManager.GetAll().Where(t => t.Tags.Contains(tag)).ToList();
        var table =  items.Select((i, idx) => new{Index = idx++, Name = i.Name, Source = i.SourceType, Tags= i.Tags}).ToList();
        ConsoleTableService.RenderTable(table, this);
        WriteCodeExample($"Rows matched by tag [{tag}]", $"{items.Count}");

        WriteHeadLine("Backup will be performed before deletion takes place.");
        var deleteConfirmation = DialogService.YesNoDialog($"Do you want to delete {items.Count} number of items from the database?");
        if (deleteConfirmation)
        {
            var fileName = DBManager.Backup();
            WriteLine($"File was first backed up to {fileName}");
            DBManager.Delete(items);
        }
        return Ok();
    }
}