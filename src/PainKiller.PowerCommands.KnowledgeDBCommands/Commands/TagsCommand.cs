using PainKiller.PowerCommands.KnowledgeDBCommands.BaseClasses;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Commands;

[PowerCommandsToolbar("<name>|[Options]|remove")]
[PowerCommandDesign(  description: "Append or remove a tag, append is default if no option is used.",
                        arguments:"!<name>",
                          options: "remove",
                          example: "//First search and select one item|//append|tag myNewTag|//Delete tag|tag --delete myTagNameToDelete")]
public class TagsCommand : DisplayCommandsBase
{
    public TagsCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }
    public override RunResult Run()
    {
        if (SelectedItem == null) return Ok();
        ToolbarService.ClearToolbar();
        var tag = Input.SingleArgument;
        if (HasOption("remove"))
        {
            Remove(SelectedItem, tag);
            return Ok();
        }
        Append(SelectedItem, tag);
        return Ok();
    }
    private void Append(KnowledgeItem item, string tags)
    {
        var items = GetAllItems();
        var match = items.First(i => i.ItemID == item.ItemID);
        match.Tags = $"{match.Tags},{tags}";
        if (!DialogService.YesNoDialog($"Are this update ok? {match.Name} {match.SourceType} {match.Tags}?")) return;
        DBManager.Edit(match);
        WriteLine($"Item {match.ItemID} {match.Name} updated.");
        Details(match);
    }

    private void Remove(KnowledgeItem item, string tags)
    {
        var items = GetAllItems();
        var match = items.First(i => i.ItemID == item.ItemID);
        match.Tags = $"{match.Tags}".Replace($",{tags}","").Replace(tags,"");
        if (!DialogService.YesNoDialog($"Are this update ok? {match.Name} {match.SourceType} {match.Tags}?")) return;
        DBManager.Edit(match);
        WriteLine($"Item {match.ItemID} {match.Name} updated.");
        Details(match);
    }
}