using PainKiller.PowerCommands.KnowledgeDBCommands.BaseClasses;
using PainKiller.PowerCommands.Shared.Attributes;
using PainKiller.PowerCommands.Shared.DomainObjects.Core;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Commands;

[PowerCommandsToolbar("Press -> [Enter]")]
[PowerCommandDesign(  description: "Open the current selected item",
                        arguments: "",
                          options: "",
                          example: "open")]
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