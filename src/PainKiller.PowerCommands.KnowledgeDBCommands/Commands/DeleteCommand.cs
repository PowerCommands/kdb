using PainKiller.PowerCommands.KnowledgeDBCommands.BaseClasses;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Commands;

[PowerCommandsToolbar("Press -> [Enter]")]
[PowerCommandDesign(  description: "Delete the current selected item",
                        arguments: "",
                          options: "",
                          example: "//First search and select one item|delete")]
public class DeleteCommand : DisplayCommandsBase
{
    public DeleteCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }
    public override RunResult Run()
    {
        if (SelectedItem != null) Delete(SelectedItem);
        return Ok();
    }
    private void Delete(KnowledgeItem item)
    {
        Console.Clear();
        Details(item);

        if (!DialogService.YesNoDialog($"Are you sure you want to delete the item?")) return;
        DBManager.Delete(item.ItemID.GetValueOrDefault());
        WriteLine($"Item {item.ItemID} {item.Name} removed.");
    }
}