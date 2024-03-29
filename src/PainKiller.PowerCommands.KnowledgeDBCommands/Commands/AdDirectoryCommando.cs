﻿using System.DirectoryServices;
#pragma warning disable CA1416

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Commands;

internal class AdDirectoryCommando(string identifier, PowerCommandsConfiguration configuration, string filter) : CommandBase<PowerCommandsConfiguration>(identifier, configuration)
{
    public override RunResult Run()
    {
        var dirEntry = Configuration.AdDomain;
        if (string.IsNullOrEmpty(filter)) return BadParameterError("Search argument can not be empty");
        try
        {
            var entry = new DirectoryEntry($"LDAP://{dirEntry}");
            var searcher = new DirectorySearcher($"*{entry}");

            searcher.Filter = $"(&(objectClass=user)(|(displayName=*{filter}*)(sn={filter})(sAMAccountName={filter})))";
            var results = searcher.FindAll();
            if (results.Count == 0)
            {
                WriteLine($"No match with [{filter}]");
                return Ok();
            }
            var result = results[0];

            var displayName = result.Properties["displayName"][0].ToString();
            var email = result.Properties["mail"][0].ToString();

            WriteSuccessLine($"{displayName} ({email})");

            var properties = Configuration.AdProperties.Split('|');
            if (properties.First() == "*")
            {
                foreach (string propertyName in result.Properties.PropertyNames)
                {
                    foreach (var propertyValue in result.Properties[propertyName]) WriteLine($"{propertyName}: {propertyValue}");
                }
            }
            else
            {
                foreach (var property in properties)
                {
                    foreach (var propertyValue in result.Properties[property]) WriteCodeExample(property, $"{propertyValue}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return Ok();
    }
}