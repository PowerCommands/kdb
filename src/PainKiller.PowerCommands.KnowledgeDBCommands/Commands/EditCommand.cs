using PainKiller.PowerCommands.KnowledgeDBCommands.BaseClasses;
using PainKiller.PowerCommands.KnowledgeDBCommands.Extensions;
using PainKiller.PowerCommands.Shared.Attributes;
using PainKiller.PowerCommands.Shared.DomainObjects.Core;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Commands;

[PowerCommandDesign(  description: "Edit selected item",
                        arguments: "",
                          options: "",
                          example: "edit")]
public class EditCommand : DisplayCommandsBase
{
    public EditCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }
    public override RunResult Run()
    {
        var selected = DialogService.ListDialog("Choose item to edit, then press enter.", Items.Select(i => i.Name).ToList());
        if (selected.Count == 0) return Ok();
        var selectedIndex = selected.First().Key;
        var selectedItem = Items[selectedIndex-1];
        
        WriteHeadLine($"\nEdit [{selectedItem.Name}]\n");
        WriteLine("Use syntax property name = <new value> for those properties of the item you want to change. Example below shows all properties you can change.");
        WriteLine("If you need to append or remove a tag, you can also do that with the [tag] command\n");
        WriteLine($"{nameof(KnowledgeItem.Name.ToLower)}=<name> {nameof(KnowledgeItem.Tags.ToLower)}=<tags> {nameof(KnowledgeItem.Uri.ToLower)}=<uri> {nameof(KnowledgeItem.SourceType.ToLower)}=<source type>\n");
        WriteLine($"Supported source types are: url, onenote, path, file\n");

        var input = DialogService.QuestionAnswerDialog("Input your edit string!");

        var editItem = input.ToItem();

        Edit(selectedItem, editItem.Name, editItem.SourceType, editItem.Tags, editItem.Uri);

        return Ok();
    }
}