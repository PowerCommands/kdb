using PainKiller.PowerCommands.Bootstrap;
using PainKiller.PowerCommands.Core.Services;

Console.CursorTop = 2;
//https://patorjk.com/software/taag/#p=display&f=Big&t=KDB%201.1
ConsoleService.Service.WriteLine(nameof(Program),@" _  _______  ____    __  __ 
 | |/ /  __ \|  _ \  /_ |/_ |
 | ' /| |  | | |_) |  | | | |
 |  < | |  | |  _ <   | | | |
 | . \| |__| | |_) |  | |_| |
 |_|\_\_____/|____/   |_(_)_|", ConsoleColor.Cyan);
ConsoleService.Service.WriteHeaderLine(nameof(Program), "\nKnowledgeDB Commands 1.1");
Startup.ConfigureServices().Run(args);