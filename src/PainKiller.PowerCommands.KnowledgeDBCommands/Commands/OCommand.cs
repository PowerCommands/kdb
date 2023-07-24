using PainKiller.PowerCommands.Shared.Attributes;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Commands;

[PowerCommandsToolbar("Press -> [Enter]")]
[PowerCommandDesign(  description: "Open the current selected item",
                        arguments: "",
                          options: "",
                          example: "//First search and select one item|open")]
public class OCommand : OpenCommand
{
    public OCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }
}