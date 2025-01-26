using PainKiller.PowerCommands.Core.Managers;
using PainKiller.PowerCommands.Core.Services;

namespace PainKiller.PowerCommands.Bootstrap.Managers;
public partial class PowerCommandsManager
{
    
    private void RunCustomCode(RunFlowManager runFlow)
    {
        if (string.IsNullOrEmpty(runFlow.Raw.Trim())) ToolbarService.ClearToolbar();
    }
}