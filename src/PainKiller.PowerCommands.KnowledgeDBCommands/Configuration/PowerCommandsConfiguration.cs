using PainKiller.PowerCommands.Configuration.DomainObjects;
using PainKiller.PowerCommands.Shared.DomainObjects.Configuration;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Configuration;

public class PowerCommandsConfiguration : CommandsConfiguration
{
    public int DisplayTagsMaxLength { get; set; } = 50;
    public int DisplayUrlMaxLength { get; set; } = 30;
    public ShellConfigurationItem ShellConfiguration { get; set; } = new();
    public readonly string DatabaseFileName = Path.Combine(ConfigurationGlobals.ApplicationDataFolder, "kdb", $"{nameof(KnowledgeDatabase)}.data");
}