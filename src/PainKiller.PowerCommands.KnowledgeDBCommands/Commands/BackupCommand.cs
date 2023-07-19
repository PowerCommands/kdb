using PainKiller.PowerCommands.Core.BaseClasses;
using PainKiller.PowerCommands.Shared.Attributes;
using PainKiller.PowerCommands.Shared.DomainObjects.Core;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Commands;

[PowerCommandsToolbar("[Enter]=backup|Options|--show (optional)")]
[PowerCommandDesign(description: "Backup your knowledge DB file to the configured path in PowerCommandsConfiguration.yaml file, use --show option to just show your already backup up files.",
                        options: "show",
                        example: "//Backup file to the configured path in PowerCommandsConfiguration.yaml file|backup|//Show backup files|backup --show")]
public class BackupCommand : CommandWithToolbarBase<PowerCommandsConfiguration>
{
    public BackupCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }
    public override RunResult Run()
    {
        if (!HasOption("show"))
        {
            if (!Directory.Exists(IPowerCommandServices.DefaultInstance!.Configuration.BackupPath)) Directory.CreateDirectory(IPowerCommandServices.DefaultInstance!.Configuration.BackupPath);
            var fileName = StorageService<KnowledgeDatabase>.Service.Backup();
            WriteLine($"File is backed up to {fileName}");
            return Ok();
        }
        ShellService.Service.OpenDirectory(IPowerCommandServices.DefaultInstance!.Configuration.BackupPath);
        return Ok();
    }
}