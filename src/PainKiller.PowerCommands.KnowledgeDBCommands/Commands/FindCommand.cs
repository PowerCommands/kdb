using PainKiller.PowerCommands.KnowledgeDBCommands.BaseClasses;
using PainKiller.PowerCommands.KnowledgeDBCommands.Extensions;
using PainKiller.PowerCommands.Shared.Attributes;
using PainKiller.PowerCommands.Shared.DomainObjects.Core;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Commands;

[PowerCommandsToolbar("Options|--year (optional)|--month (optional, needs year)")]
[PowerCommandDesign(  description: "Find a knowledge item, use the index value to open, edit or delete it",
                        arguments: "<SearchPhrase>",
                          options: "!year|!month",
                          example: "//Do a simple search|find <search phrase>|//Search with two phrases, findings must contain both|find <search phrase1> <search phrase2>|//Search a phrase created a certain year|find <search phrase> --year 2022|//Search a phrase created a certain year and month|find <search phrase> --year 2022 --month 3")]
public class FindCommand : DisplayCommandsBase
{
    public FindCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }
    public override RunResult Run()
    {
        Items = GetDb().Items.Where(i => i.Name.ToLower().Contains(Input.SingleArgument.ToLower()) || i.Tags.ToLower().Contains(Input.SingleArgument.ToLower()) || i.Uri.ToLower().Contains(Input.SingleArgument.ToLower())).OrderByDescending(i => i.Created).ToList();
        if (Input.Arguments.Length > 1)
        {
            for (int i = 1; i < Input.Arguments.Length; i++) Items = Items.Where(m => m.Name.ToLower().Contains(Input.Arguments[i].ToLower()) || m.Tags.ToLower().Contains(Input.Arguments[i].ToLower()) || m.Uri.ToLower().Contains(Input.Arguments[i])).OrderByDescending(i => i.Created).ToList();
        }
        var year = Input.OptionToInt("year");
        var month = Input.OptionToInt("month");
        if (year > 0)
        {
            var argument = Input.SingleArgument.Replace($"{year}", "").Replace($"{month}","");
            if (string.IsNullOrEmpty(argument)) Items = GetDb().Items;
            Items = Items.Where(i => i.Created.Year == year && (i.Created.Month == month) || month == 0).ToList();
        }
        if (Items.Count == 1 && Configuration.ShellConfiguration.Autostart) Open(Items.First());
        ShowResult();
        return Ok();
    }
    public override void RunCompleted() { }
    protected void ShowResult()
    {
        WriteHeadLine($"Found {Items.Count} matches.");
        var selected = DialogService.ListDialog("Enter a valid number to select item, or just hit enter to cancel.", Items.Select(i => $"{i.Name} {i.SourceType} {i.Uri.Display(Configuration.DisplayUrlMaxLength)} {i.Tags.Display(Configuration.DisplayTagsMaxLength)}").ToList());
        if (selected.Count == 0) return;
        var selectedIndex = selected.First().Key;
        SelectedItem = Items[selectedIndex];

        ToolbarService.DrawToolbar(new []{$"[Action] ->","open (CTRL+O)","edit","delete","tags" });
    }
}