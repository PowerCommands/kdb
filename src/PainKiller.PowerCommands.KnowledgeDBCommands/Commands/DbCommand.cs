using PainKiller.PowerCommands.Configuration.DomainObjects;
using PainKiller.PowerCommands.Core.BaseClasses;
using PainKiller.PowerCommands.Shared.Attributes;
using PainKiller.PowerCommands.Shared.DomainObjects.Core;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Commands;

[PowerCommandsToolbar("Press -> [Enter]")]
[PowerCommandTest(         tests: " ")]
[PowerCommandDesign( description: "Open the database file with your configured editor.",
                         example: "db")]
public class DbCommand : CommandWithToolbarBase<PowerCommandsConfiguration>
{
    public DbCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override RunResult Run()
    {
        try
        {
            var fileName = Path.Combine(Configuration.DatabaseFileName, $"{nameof(KnowledgeDatabase)}.data");
            ShellService.Service.Execute(Configuration.CodeEditor, arguments: fileName, ConfigurationGlobals.ApplicationDataFolder, WriteLine, fileExtension: "");
        }
        catch (Exception) { return BadParameterError("Your editor must be included in Path environment variables"); }
        return Ok();
    }
}