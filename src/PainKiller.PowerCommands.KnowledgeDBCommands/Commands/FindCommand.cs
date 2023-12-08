using PainKiller.PowerCommands.KnowledgeDBCommands.BaseClasses;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Commands;

[PowerCommandsToolbar("Options|--year (optional)|--month (optional, needs year)")]
[PowerCommandDesign(description: "Find a knowledge item, use the index value to open, edit or delete it",
                        arguments: "<SearchPhrase>",
                          options: "!year|!month",
                          example: "//Do a simple search|find <search phrase>|//Search with two phrases, findings must contain both|find <search phrase1> <search phrase2>|//Search a phrase created a certain year|find <search phrase> --year 2022|//Search a phrase created a certain year and month|find <search phrase> --year 2022 --month 3")]
public class FindCommand : DisplayCommandsBase
{
    public FindCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }
    public override RunResult Run()
    {
        Items = GetAllItems().Where(i => i.Name.ToLower().Contains(Input.SingleArgument.ToLower()) || i.Tags.ToLower().Contains(Input.SingleArgument.ToLower()) || i.Uri.ToLower().Contains(Input.SingleArgument.ToLower())).OrderByDescending(i => i.Updated).ToList();
        var year = Input.OptionToInt("year");
        var month = Input.OptionToInt("month");
        var arguments = Input.Arguments.ToList();
        if (HasOption("year")) arguments = Input.Arguments.Where(a => a != $"{year}").ToList();
        if (HasOption("month")) arguments = arguments.Where(a => a != $"{month}").ToList();
        if (Input.Arguments.Length > 1)
        {
            //Add filters as many as the user have given, but limit the filter count to 100, higher value must some kind of abuse.
            var iterations = Input.Arguments.Length < 100 ? Input.Arguments.Length : 100;
            for (var i = 1; i < iterations; i++) Items = Items.Where(item => item.Name.ToLower().Contains(arguments[i].ToLower()) || item.Tags.ToLower().Contains(arguments[i].ToLower()) || item.Uri.ToLower().Contains(arguments[i])).OrderByDescending(k => k.Updated).ToList();
        }
        if (year > 0)
        {
            Items = Items.Where(i => i.Created.Year == year && (i.Created.Month == month || month == 0)).ToList();
        }
        ShowResult($"Found {Items.Count} matches.");
        if (Items.Count == 1 && Configuration.ShellConfiguration.Autostart) Open(Items.First(), writePrompt: false);
        return Ok();
    }
    public override void RunCompleted() { }
}