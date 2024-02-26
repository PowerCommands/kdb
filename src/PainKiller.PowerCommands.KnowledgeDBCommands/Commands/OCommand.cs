using PainKiller.PowerCommands.Configuration.DomainObjects;
using PainKiller.PowerCommands.ReadLine;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Commands;

[PowerCommandsToolbar("Press -> [Enter]")]
[PowerCommandDesign(  description: "Open the current selected item",
    arguments: "",
    options: "",
    example: "//First search and select one item|open")]
public class OCommand : OpenCommand
{
    public OCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) => ReadLineService.OpenShortCutPressed += () =>
    {
        Run();
        ToolbarService.ClearToolbar();
        ConsoleService.Service.ClearRow(Console.CursorTop);
        Write(ConfigurationGlobals.Prompt);
    };
}