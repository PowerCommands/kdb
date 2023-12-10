using PainKiller.PowerCommands.KnowledgeDBCommands.BaseClasses;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Commands;

[PowerCommandsToolbar("Press -> [Enter]")]
[PowerCommandDesign(  description: "Open the current selected items, max 2 is opened at the same time.",
                        arguments: "",
                          options: "",
                          example: "//First search and select one item|open")]
public class OpenCommand(string identifier, PowerCommandsConfiguration configuration) : DisplayCommandsBase(identifier, configuration)
{
    public override RunResult Run()
    {
        foreach (var item in SelectedItems.Take(2)) Open(item);
        ToolbarService.ClearToolbar();
        return Ok();
    }
}