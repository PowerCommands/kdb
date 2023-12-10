using PainKiller.PowerCommands.Configuration.DomainObjects;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Commands;

[PowerCommandDesign(description: "Restore your database file from a previous backup",
                        example: "restore")]
public class RestoreCommand : CommandBase<PowerCommandsConfiguration>
{
    public RestoreCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }
    public override RunResult Run()
    {
        var backupPath = IPowerCommandServices.DefaultInstance!.Configuration.BackupPath;
        if (!Directory.Exists(backupPath))
        {
            WriteLine($"No backup directory exist, backup path must be configured in the {ConfigurationGlobals.MainConfigurationFile}");
            return Ok();
        }
        var directoryInfo = new DirectoryInfo(backupPath);

        var files = directoryInfo.GetFiles("KnowledgeDatabase-*.data").OrderByDescending(f => f.CreationTime).Select(f => $"{f.Name} {f.CreationTime} {f.Length}").ToList();
        var selectedFileItem = ListService.ListDialog("Choose one file to restore from?", files);

        if (selectedFileItem.Count == 0)
        {
            WriteLine("Restore operation cancelled by user.");
            return Ok();
        }
        var fileName = Path.Combine(backupPath, selectedFileItem.FirstOrDefault().Value.Split(' ').First());

        var confirmRestore = DialogService.YesNoDialog($"Do you want to restore the database with this backup file? (this can not be undone)");
        if (confirmRestore)
        {
            File.Copy(fileName, Configuration.DatabaseFileName, overwrite: true);
            WriteSuccessLine($"Database file {Configuration.DatabaseFileName} overwritten with {fileName} OK");
        }
        else WriteLine("Restore operation cancelled by user.");
        return Ok();
    }
}