using PainKiller.PowerCommands.Shared.DomainObjects.Configuration;
namespace PainKiller.PowerCommands.KnowledgeDBCommands.Managers;
public class InfoPanelManager(InfoPanelConfiguration configuration) : InfoPanelManagerBase(configuration, new UserNameInfoPanelContent(), new TimeInfoPanelContent(), new CurrentDirectoryInfoPanel());