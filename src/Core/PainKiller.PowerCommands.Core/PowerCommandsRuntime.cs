﻿namespace PainKiller.PowerCommands.Core
{
    public class PowerCommandsRuntime<TConfig> : IPowerCommandsRuntime where TConfig : CommandsConfiguration
    {
        private readonly TConfig _configuration;
        private readonly IDiagnosticManager _diagnostic;
        public List<IConsoleCommand> Commands { get; } = new();
        public PowerCommandsRuntime(TConfig configuration, IDiagnosticManager diagnosticManager)
        {
            _configuration = configuration;
            _diagnostic = diagnosticManager;
            Initialize();
        }
        private void Initialize()
        {
            ReflectionService.Service.DesignAttributeReflected += Service_DesignAttributeReflected;
            foreach (var component in _configuration.Components)
            {
                Commands.AddRange(ReflectionService.Service.GetCommands(component, _configuration));
                if (!_configuration.ShowDiagnosticInformation) continue;
                _diagnostic.Header($"\nFound commands in component: {component.Name}");
                foreach (var consoleCommand in Commands) _diagnostic.Message(consoleCommand.Identifier);
            }

            foreach (var proxyCommand in _configuration.ProxyCommands)
            {
                foreach (var command in proxyCommand.Commands)
                {
                    var identifiers = command.Split(ConfigurationGlobals.ArraySplitter);
                    var identifier = identifiers[0];
                    var identifierAlias = command.Contains($"|") ? identifiers[1] : identifier;
                    ConsoleService.Service.WriteLine("PowerCommandsRuntime", $"Proxy command [{identifierAlias}] added");
                    var powerCommand = new ProxyCommando(identifier, _configuration, proxyCommand.Name, proxyCommand.WorkingDirctory, identifierAlias);

                    var suggestions = new List<string> { "--retry-interval-seconds", "--no-quit", "--help" };
                    var commandDesignOverrides = _configuration.CommandDesignOverrides.FirstOrDefault(c => c.Name == identifier);
                    if (commandDesignOverrides != null) suggestions.AddRange(commandDesignOverrides.Suggestions.Split('|'));
                    SuggestionProviderManager.AddContextBoundSuggestions(identifierAlias, suggestions.ToArray());
                    if (Commands.All(c => c.Identifier != powerCommand.Identifier)) Commands.Add(powerCommand);
                    else ConsoleService.Service.WriteWarning("PowerCommandsRuntime", $"A command with the same identifier [{command}] already exist, proxy command not added.");
                }
            }
            IPowerCommandsRuntime.DefaultInstance = this;
        }
        public string[] CommandIDs => Commands.Select(c => c.Identifier).ToArray();
        public RunResult ExecuteCommand(string rawInput)
        {
            var input = rawInput.Interpret(_configuration.DefaultCommand);
            var command = Commands.FirstOrDefault(c => c.Identifier.ToLower() == input.Identifier);
            if (command == null && !string.IsNullOrEmpty(_configuration.DefaultCommand))
            {
                input = $"{_configuration.DefaultCommand} {rawInput}".Interpret();
                command = Commands.FirstOrDefault(c => c.Identifier.ToLower() == input.Identifier); //Retry with default command if no command found on the first try
            }
            if (command == null) throw new ArgumentOutOfRangeException($"Could not identify any Commmand with identy {input.Identifier}");

            var attrib = command.GetPowerCommandAttribute();
            if (input.Options.Any(f => f == "--help"))
            {
                if (!attrib.OverrideHelpOption)
                {
                    HelpService.Service.ShowHelp(command, clearConsole: true);
                    command.RunCompleted();
                    return new RunResult(command, input, "User prompted for help with --help option", RunResultStatus.Ok);
                }
            }
            if (command.InitializeAndValidateInput(input, attrib))
            {
                Latest = new RunResult(command, input, "Validation error", RunResultStatus.InputValidationError);
                command.RunCompleted();
                return Latest;
            }

            if (command.GetPowerCommandAttribute().UseAsync.GetValueOrDefault()) return ExecuteAsyncCommand(command, input);
            try
            {
                Latest = command.Run();
                if (!attrib.DisableProxyOutput) StorageService<ProxyResult>.Service.StoreObject(new ProxyResult { Identifier = Latest.Input.Identifier, Raw = Latest.Input.Raw, Output = Latest.Output, Status = Latest.Status }, command.GetOutputFilename());
            }
            catch (Exception e) { Latest = new RunResult(command, input, e.Message, RunResultStatus.ExceptionThrown); }
            finally { command.RunCompleted(); }
            return Latest;
        }
        public RunResult ExecuteAsyncCommand(IConsoleCommand command, ICommandLineInput input)
        {
            try
            {
                command.RunAsync().ContinueWith((_) =>
                {
                    command.RunCompleted();
                    ConsoleService.Service.WritePrompt();
                });
                Latest = new RunResult(command, input, "Command running async operation", RunResultStatus.Async);
            }
            catch (Exception e)
            {
                Latest = new RunResult(command, input, e.Message, RunResultStatus.ExceptionThrown);
            }
            return Latest;
        }
        public RunResult? Latest { get; private set; }

        #region event listeners
        private void Service_DesignAttributeReflected(object? sender, Shared.Events.DesignAttributeReflectedArgs e)
        {
            var suggestions = new List<string>();
            if (!string.IsNullOrEmpty(e.DesignAttribute.Options)) suggestions.AddRange(e.DesignAttribute.Options.Split(ConfigurationGlobals.ArraySplitter).Select(f => $"--{f}"));
            suggestions.Add("--help");
            if (!string.IsNullOrEmpty(e.DesignAttribute.Suggestions)) suggestions.AddRange(e.DesignAttribute.Suggestions.Split(ConfigurationGlobals.ArraySplitter).Select(f => $"{f}"));
            SuggestionProviderManager.AddContextBoundSuggestions(e.Command.Identifier, suggestions.ToArray());
        }
        #endregion
    }
}