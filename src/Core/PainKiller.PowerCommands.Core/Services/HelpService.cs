﻿using PainKiller.PowerCommands.Shared.Extensions;

namespace PainKiller.PowerCommands.Core.Services
{
    public class HelpService : IHelpService
    {
        private const bool WriteToLog = false;
        private HelpService() { }
        private static readonly Lazy<IHelpService> Lazy = new(() => new HelpService());
        public static IHelpService Service => Lazy.Value;
        public void ShowHelp(IConsoleCommand command, bool clearConsole = true)
        {
            var da = command.GetPowerCommandAttribute();
            if (clearConsole) ConsoleService.Service.Clear();

            var examples = da.Examples.Split(ConfigurationGlobals.ArraySplitter);

            ConsoleService.Service.WriteHeaderLine($"{GetType().Name}", $"{command.Identifier}\n\nDescription", writeLog: WriteToLog);
            ConsoleService.Service.WriteLine(nameof(HelpService), $" {da.Description.Replace("[PIPE]", "|")}");
            Console.WriteLine();

            var args = da.Arguments.Replace("!", "").Split(ConfigurationGlobals.ArraySplitter);
            var quotes = da.Quotes.Replace("!", "").Split(ConfigurationGlobals.ArraySplitter);
            var options = da.Options.Replace("!", "").Split(ConfigurationGlobals.ArraySplitter);

            ConsoleService.Service.WriteHeaderLine(nameof(HelpService), "Usage");

            var argsMarkup = args.Any(a => !string.IsNullOrEmpty(a)) ? "[arguments]" : "";
            var quotesMarkup = quotes.Any(q => !string.IsNullOrEmpty(q)) ? "[quotes]" : "";
            var optionMarkup = options.Any(f => !string.IsNullOrEmpty(f)) ? "[options]" : "";

            ConsoleService.Service.Write(nameof(HelpService), $" {command.Identifier}", ConsoleColor.Blue);
            ConsoleService.Service.WriteLine(nameof(HelpService), $" {argsMarkup} {quotesMarkup} {optionMarkup}");
            ConsoleService.Service.WriteLine(nameof(HelpService), "");
            ConsoleService.Service.WriteHeaderLine(nameof(HelpService), "Options:");
            var optionDescriptions = options.Select(f => f.ToOptionDescription());
            ConsoleService.Service.WriteLine(nameof(HelpService), $" {string.Join(',', optionDescriptions)}");
            Console.WriteLine("");
            if (string.IsNullOrEmpty(da.Examples)) return;

            ConsoleService.Service.WriteHeaderLine($"{GetType().Name}", $"{nameof(da.Examples)}:", writeLog: WriteToLog);
            foreach (var e in examples) WriteItem(e.Replace("[PIPE]", "|"), command.Identifier);
        }
        private void WriteItem(string description, string identifier = "")
        {
            var formatedDescription = description.Replace("[PIPE]", "|");
            if (formatedDescription.StartsWith("//"))
            {
                ConsoleService.Service.WriteHeaderLine($"{GetType().Name}", $"\n{formatedDescription.Replace("//", "")}", ConsoleColor.White);
                return;
            }
            ConsoleService.Service.Write(GetType().Name, $" {identifier} ", ConsoleColor.Blue, WriteToLog);
            ConsoleService.Service.WriteLine(GetType().Name, formatedDescription.Replace(identifier, "").Trim(), null, WriteToLog);
        }
    }
}