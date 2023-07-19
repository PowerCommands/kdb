using PainKiller.PowerCommands.KnowledgeDBCommands.BaseClasses;
using PainKiller.PowerCommands.Shared.Attributes;
using PainKiller.PowerCommands.Shared.DomainObjects.Core;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Commands;

[PowerCommandsToolbar("Press -> [Enter]")]
[PowerCommandDesign(  description: "Delete the current selected item",
                        arguments: "",
                          options: "",
                          example: "delete")]
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
        
        var db = GetDb();
        var match = db.Items.First(i => i.ItemID == item.ItemID);
        db.Items.Remove(match);
        Save(db);
        
        WriteLine($"Item {item.ItemID} {item.Name} removed.");
    }

    
}