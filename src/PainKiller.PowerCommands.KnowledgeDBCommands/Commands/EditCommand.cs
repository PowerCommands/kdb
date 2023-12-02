using PainKiller.PowerCommands.KnowledgeDBCommands.BaseClasses;
using PainKiller.PowerCommands.KnowledgeDBCommands.Extensions;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Commands;

[PowerCommandsToolbar("Press -> [Enter]")]
[PowerCommandDesign(  description: "Edit selected item",
                        arguments: "",
                          options: "",
                          example: "//First search and select one item|edit")]
public class EditCommand : DisplayCommandsBase
{
    public EditCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }
    public override RunResult Run()
    {
        if (SelectedItem == null) return Ok();
        Console.Clear();
        WriteHeadLine($"\nEdit [{SelectedItem.Name}]\n");
        Details(SelectedItem);
        WriteLine("\nUse syntax property name=<new value> for those properties of the item you want to change. Example below shows all properties you can change.");
        WriteLine("If you need to append or remove a tag, you can also do that with the [tags] command\n");
        WriteLine($"{nameof(KnowledgeItem.Name).ToLower()}=<name>|{nameof(KnowledgeItem.Tags).ToLower()}=<tags>|{nameof(KnowledgeItem.Uri).ToLower()}=<uri>|{nameof(KnowledgeItem.SourceType).ToLower()}=<source type>\n");
        WriteLine($"Supported source types are: url, onenote, path, file\n");

        var input = DialogService.QuestionAnswerDialog("Input your edit string!");

        var editItem = input.ToItem();
        Edit(SelectedItem, editItem.Name, editItem.SourceType, editItem.Tags, editItem.Uri);

        return Ok();
    }
    private void Edit(KnowledgeItem item, string name, string source, string tags, string url = "")
    {
        var db = GetDb();
        var match = db.Items.First(i => i.ItemID == item.ItemID);
        match.Updated = DateTime.Now;
        db.Items.Remove(match);

        if (!string.IsNullOrEmpty(name)) match.Name = name;
        if (!string.IsNullOrEmpty(source) && "url path onenote".Contains(source)) match.SourceType = source;
        if (!string.IsNullOrEmpty(tags)) match.Tags = tags;
        if (!string.IsNullOrEmpty(url)) match.Uri = url;
        if (!DialogService.YesNoDialog($"Are this update ok?\n{match}\n")) return;
        db.Items.Add(match);
        Save(db);
        WriteLine($"Item {match.ItemID} {match.Name} updated.");
        ToolbarService.ClearToolbar();
    }
}