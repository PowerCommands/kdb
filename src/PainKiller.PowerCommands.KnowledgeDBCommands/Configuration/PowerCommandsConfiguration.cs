using PainKiller.PowerCommands.Configuration.DomainObjects;
using PainKiller.PowerCommands.Shared.DomainObjects.Configuration;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Configuration;

public class PowerCommandsConfiguration : CommandsConfiguration
{
    public int DisplayTagsMaxLength { get; set; } = 50;
    public int DisplayUrlMaxLength { get; set; } = 30;
    public int OpenAiMaxTokens { get; set; } = 50;
    public string OpenAiApiUrl { get; set; } = "https://api.openai.com/v1/engines/gpt-3.5-turbo/completions";
    public string AdDomain { get; set; } = "mydomain.local";
    public string AdProperties { get; set; } = "displayName|mail";
    public ShellConfigurationItem Shell { get; set; } = new();
    public SshConfiguration Ssh { get; set; } = new();
    public readonly string DatabaseFileName = Path.Combine(ConfigurationGlobals.ApplicationDataFolder, $"{nameof(KnowledgeDatabase)}.data");
    public readonly string FileTypesFileName = Path.Combine(AppContext.BaseDirectory, "KnowledgeItemFileTypes.json");
}