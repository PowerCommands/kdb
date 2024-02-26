using PainKiller.PowerCommands.KnowledgeDBCommands.BaseClasses;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Commands;

[PowerCommandsToolbar("[Options]|--days <number>|--weeks <number>|Source type one of (--url,--onenote,--path,--file")]
[PowerCommandDesign(description: "List the latest added knowledge documents.",
                        options: "opened|!days|!weeks|url|path|onenote|file",
                        example: "//Show created items the last 3 days|latest --days 3|//Show created items the last 4 weeks.|latest --weeks 4|//Show all created files the last week|latest --week 1 --file")]
public class LatestCommand(string identifier, PowerCommandsConfiguration configuration) : DisplayCommandsBase(identifier, configuration)
{
    public override RunResult Run()
    {
        var sourceType = Input.GetOptionValue(new[] { "url", "onenote", "path", "file" });
        var dayFactor = Input.HasOption("weeks") ? 7 : 1;
        var number = 0;
        if (int.TryParse(Input.GetOptionValue("weeks"), out var index)) number = index;
        if (int.TryParse(Input.GetOptionValue("days"), out var index2)) number = index2;

        var latestDate = number == 0 ? DateTime.Now.AddDays(-7) : DateTime.Now.AddDays(-(dayFactor*number));
        if (HasOption("opened"))
        {
            var takeCount = Input.OptionToInt("opened", 10);
            Items = GetAllItems().OrderByDescending(i => i.Updated).Take(takeCount).ToList();
            ShowResult($"Latest {Items.Count} opened items.");
            return Ok();
        }
        Items = GetAllItems().Where(i => i.Created > latestDate && (i.SourceType == sourceType || string.IsNullOrEmpty(sourceType))).ToList();
        ShowResult($"Latest added {Items.Count} matches since {latestDate.ToShortDateString()}.");

        return Ok();
    }
}