using PainKiller.PowerCommands.KnowledgeDBCommands.BaseClasses;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Commands;

[PowerCommandsToolbar("Press -> [Enter]")]
[PowerCommandDesign(  description: "Open the current selected item",
                        arguments: "",
                          options: "",
                          example: "//First search and select one item|open")]
public class OpenCommand : DisplayCommandsBase
{
    public OpenCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }
    public override RunResult Run()
    {
        if (SelectedItem != null) Open(SelectedItem);
        ToolbarService.ClearToolbar();
        return Ok();
    }
}