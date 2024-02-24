using PainKiller.PowerCommands.KnowledgeDBCommands.BaseClasses;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Commands;

[PowerCommandsToolbar("<name>|[Options]|remove")]
[PowerCommandDesign(  description: "Append or remove a tag to selected item(s), append is default if no option is used.",
                        arguments:"!<name>",
                          options: "remove",
                          example: "//First search and select one item|//append|tag myNewTag|//Delete tag|tag --delete myTagNameToDelete")]
public class TagsCommand(string identifier, PowerCommandsConfiguration configuration) : DisplayCommandsBase(identifier, configuration)
{
    public override RunResult Run()
    {
        if (SelectedItems.Count == 0) return Ok();
        Console.Clear();
        ToolbarService.ClearToolbar();
        ShowSelectedItems();
        var tag = Input.SingleArgument;
        if (HasOption("remove"))
        {
            foreach (var item in SelectedItems) Remove(item, tag, SelectedItems.Count == 1);
            return Ok();
        }
        foreach (var item in SelectedItems) Append(item, tag, SelectedItems.Count == 1);
        return Ok();
    }
    private void Append(KnowledgeItem item, string tags, bool confirm = true)
    {
        var items = GetAllItems();
        var match = items.First(i => i.ItemID == item.ItemID);
        match.Tags = $"{match.Tags},{tags}";
        if(confirm) if (!DialogService.YesNoDialog($"Are this update ok? {match.Name} {match.SourceType} {match.Tags}?")) return;
        DBManager.Edit(match);
        WriteLine($"Item {match.ItemID} {match.Name} updated.");
        if(confirm) Details(match);
    }

    private void Remove(KnowledgeItem item, string tags, bool confirm = true)
    {
        var items = GetAllItems();
        var match = items.First(i => i.ItemID == item.ItemID);
        match.Tags = $"{match.Tags}".Replace($",{tags}","").Replace(tags,"");
        if(confirm) if (!DialogService.YesNoDialog($"Are this update ok? {match.Name} {match.SourceType} {match.Tags}?")) return;
        DBManager.Edit(match);
        WriteLine($"Item {match.ItemID} {match.Name} updated.");
        if(confirm) Details(match);
    }
}