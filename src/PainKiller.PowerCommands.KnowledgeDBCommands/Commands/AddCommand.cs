﻿using PainKiller.PowerCommands.KnowledgeDBCommands.BaseClasses;
using PainKiller.PowerCommands.ReadLine;
using PainKiller.PowerCommands.Shared.Attributes;
using PainKiller.PowerCommands.Shared.DomainObjects.Core;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Commands;

[PowerCommandDesign(  description: "Add a new knowledge item",
                        arguments: "!<type>",
                          options: "!name|!tags",
                      suggestions: "url|onenote|path|file",
                          example: "//Add url|add url \"https://wiki/wikis\" --name \"WikiStart\" --tags wiki,start|//Add path|add path \"C:\\temp\\project\" --name \"project directory\" --tags document")]

public class AddCommand : DisplayCommandsBase
{
    public AddCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) => ReadLineService.CmdLineTextChanged += ReadLineService_CmdLineTextChanged;
    private void ReadLineService_CmdLineTextChanged(object? sender, ReadLine.Events.CmdLineTextChangedArgs e)
    {
        if(!e.CmdText.StartsWith("add")) return;
        var args = e.CmdText.Split(' ');
        var labels = new []{"Press [Space]"};
        switch (args.Length)
        {
            case 2:
                labels = new[] { "<type> (press [tab])","(url,onenote,path,file)"};
                break;
            case 3:
                labels = new[] { args[1],"\"<value>\""};
                break;
            case 4:
                labels = new []{"[Option] (press - then [tab])","--name"};
                break;
            case 5:
                labels = new []{"[Option]","--name","\"<value>\""};
                break;
            case 6:
                labels = new []{"[Option] (press - then [tab])","--tags"};
                break;
            case 7:
                labels = new []{"[Option]","--tags","<comma separated values>"};
                break;
        }
        ToolbarService.DrawToolbar(labels);
    }
    public override RunResult Run()
    {
        var item = new KnowledgeItem(Input);
        
        Details(item);
        
        if (!DialogService.YesNoDialog("Is this information correct?")) return Ok();

        var db = GetDb();
        if (db.Items.Any(i => i.Name == item.Name && i.SourceType == item.SourceType))
        {
            WriteLine($"Warning, there is already a item with the same name and source stored in DB");
            return Ok();
        }

        db.Items.Add(item);
        Save(db);

        WriteLine($"Item {item.Name} has successfully been added.");
        return Ok();
    }
}