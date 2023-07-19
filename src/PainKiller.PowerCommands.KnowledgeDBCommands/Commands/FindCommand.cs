using PainKiller.PowerCommands.KnowledgeDBCommands.BaseClasses;
using PainKiller.PowerCommands.KnowledgeDBCommands.Extensions;
using PainKiller.PowerCommands.Shared.Attributes;
using PainKiller.PowerCommands.Shared.DomainObjects.Core;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Commands;

[PowerCommandDesign(  description: "Find a knowledge item, use the index value to open, edit or delete it",
                        arguments: "<SearchPhrase>",
                          options: "",
                          example: "//Show the latest added documents|find --latest|//Show all documents|find --latest *|//Find something you are looking for, you can use two search arguments, the second argument is a filter on whats found with the first argument.|find mySearh|//Find something you are looking for, you can use two search arguments, the second argument is a filter on whats found with the first argument.|find mySearch myFilter|//Open a document from the latest search with the provided index|find 0|//Delete a document from the latest search with the provided index/|find 0 --delete|//Append tag(s) for a document from the latest search with the provided index|find 0 --append --tags addMyTags|//Edit the document from the latest search with the provided index|find 0 --edit --tags myNewTags --name myNewName --source MyNewSource-url-path-onenote|//Note that find is default command and could be omitted|//So you could just write like this to open the second item listen in the latest search|1|//If autostart is enabled and the search just find 1 item, that item will be opened automatically")]
public class FindCommand : DisplayCommandsBase
{
    public FindCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }
    public override RunResult Run()
    {
        Items = Storage.GetObject().Items.Where(i => i.Name.ToLower().Contains(Input.SingleArgument.ToLower()) || i.Tags.ToLower().Contains(Input.SingleArgument.ToLower()) || i.Uri.ToLower().Contains(Input.SingleArgument.ToLower())).OrderByDescending(i => i.Created).ToList();
        if (Input.Arguments.Length > 1)
        {
            for (int i = 1; i < Input.Arguments.Length; i++) Items = Items.Where(m => m.Name.ToLower().Contains(Input.Arguments[i].ToLower()) || m.Tags.ToLower().Contains(Input.Arguments[i].ToLower()) || m.Uri.ToLower().Contains(Input.Arguments[i])).OrderByDescending(i => i.Created).ToList();
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

        ToolbarService.DrawToolbar(new []{$"[Action] ->","open","edit","delete","tags" });
    }
}