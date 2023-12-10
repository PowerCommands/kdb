using PainKiller.PowerCommands.KnowledgeDBCommands.BaseClasses;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Commands;


[PowerCommandDesign(description: "Find a knowledge item, use the index value to open, edit or delete it",
    arguments: "<SearchPhrase>",
    options: "!year|!month",
    example: "//Do a simple search|find <search phrase>|//Search with two phrases, findings must contain both|find <search phrase1> <search phrase2>|//Search a phrase created a certain year|find <search phrase> --year 2022|//Search a phrase created a certain year and month|find <search phrase> --year 2022 --month 3")]

public class SelectedCommand : DisplayCommandsBase
{
    public SelectedCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override RunResult Run()
    {
        Console.Clear();
        WriteHeadLine("Current selected items");
        ConsoleTableService.RenderTable(SelectedItems.Select(i => new{Name = i.Name, Source = i.SourceType, Created = i.Created, Tags = i.Tags}), this);
        ToolbarService.DrawToolbar(new[] { $"[Action] ->", "open (CTRL+O)", "edit", "delete", "tags" });
        return Ok();
    }
}