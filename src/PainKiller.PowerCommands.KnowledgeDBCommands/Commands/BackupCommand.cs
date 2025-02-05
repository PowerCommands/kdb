using PainKiller.PowerCommands.KnowledgeDBCommands.BaseClasses;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Commands;

[PowerCommandsToolbar(["[Enter]=backup","Options","--show (optional)"])]
[PowerCommandDesign(description: "Backup your knowledge DB file to the configured path in PowerCommandsConfiguration.yaml file, use --show option to just show your already backup up files.",
                        options: "show",
                        example: "//Backup file to the configured path in PowerCommandsConfiguration.yaml file|backup|//Show backup files|backup --show")]
public class BackupCommand(string identifier, PowerCommandsConfiguration configuration) : DbCommandBase(identifier, configuration)
{
    public override RunResult Run()
    {
        if (!HasOption("show"))
        {
            var fileName = DBManager.Backup();
            WriteLine($"File is backed up to {fileName}");
            return Ok();
        }
        ShellService.Service.OpenDirectory(IPowerCommandServices.DefaultInstance!.Configuration.BackupPath);
        return Ok();
    }
}