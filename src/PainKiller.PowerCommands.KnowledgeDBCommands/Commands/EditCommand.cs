using PainKiller.PowerCommands.KnowledgeDBCommands.BaseClasses;
using PainKiller.PowerCommands.KnowledgeDBCommands.Extensions;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Commands;

[PowerCommandsToolbar("Press -> [Enter]")]
[PowerCommandDesign(  description: "Edit selected item",
                        arguments: "",
                          options: "",
                          example: "//First search and select one item|edit")]
public class EditCommand(string identifier, PowerCommandsConfiguration configuration) : DisplayCommandsBase(identifier, configuration)
{
    public override RunResult Run()
    {
        var selected = SelectedItems.FirstOrDefault();
        if (selected == null) return Ok();
        Console.Clear();
        WriteHeadLine($"\nEdit [{selected.Name}]\n");
        Details(selected);
        WriteLine("\nUse syntax property name=<new value> for those properties of the item you want to change. Example below shows all properties you can change.");
        WriteLine("If you need to append or remove a tag, you can also do that with the [tags] command\n");
        WriteLine($"{nameof(KnowledgeItem.Name).ToLower()}=<name>|{nameof(KnowledgeItem.Tags).ToLower()}=<tags>|{nameof(KnowledgeItem.Uri).ToLower()}=<uri>|{nameof(KnowledgeItem.SourceType).ToLower()}=<source type>\n");
        WriteLine($"Supported source types are: url, onenote, path, file\n");

        var input = DialogService.QuestionAnswerDialog("Input your edit string!");

        var editItem = input.ToItem();
        Edit(selected, editItem.Name, editItem.SourceType, editItem.Tags, editItem.Uri);

        return Ok();
    }
    private void Edit(KnowledgeItem item, string name, string source, string tags, string url = "")
    {
        var match = DBManager.First(item.ItemID.GetValueOrDefault());

        if (!string.IsNullOrEmpty(name)) match.Name = name;
        if (!string.IsNullOrEmpty(source) && "url path onenote".Contains(source)) match.SourceType = source;
        if (!string.IsNullOrEmpty(tags)) match.Tags = tags;
        if (!string.IsNullOrEmpty(url)) match.Uri = url;
        if (!DialogService.YesNoDialog($"Are this update ok?\n{match}\n")) return;
        DBManager.Edit(match);
        
        WriteLine($"Item {match.ItemID} {match.Name} updated.");
        ToolbarService.ClearToolbar();
    }
}