﻿namespace PainKiller.PowerCommands.KnowledgeDBCommands.Commands;

[PowerCommandDesign(description: "Starts up a new ssh session.",
                        example: "ssh")]
public class SshCommand(string identifier, PowerCommandsConfiguration configuration) : CommandBase<PowerCommandsConfiguration>(identifier, configuration)
{
    public override RunResult Run()
    {
        
        var ssh = Configuration.Ssh;
        ShellService.Service.Execute(Input.Identifier, $"-p {ssh.Port} {ssh.User}@{ssh.Host}", Environment.CurrentDirectory, ReadLine, "", waitForExit: false, useShellExecute: true);
        WriteLine($"{LastReadLine}");
        return Ok();
    }
}