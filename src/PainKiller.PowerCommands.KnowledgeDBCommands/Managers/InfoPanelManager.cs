using System.Drawing;
using PainKiller.PowerCommands.Shared.DomainObjects.Configuration;
using PainKiller.PowerCommands.Shared.Extensions;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Managers;

public class InfoPanelManager(InfoPanelConfiguration configuration) : IDisposable, IInfoPanelManager
{
    private Task? _infoPanelTask;
    private readonly CancellationTokenSource _cts = new();

    public void StartInfoPanelAsync()
    {
        _infoPanelTask =  RunInfoPanelAsync(_cts.Token);
    }
    public void StopUpdateReservedAreaAsync()
    {
        _cts.Cancel();
        _infoPanelTask?.Wait();
    }
    private async Task RunInfoPanelAsync(CancellationToken token)
    {
        var previousWidth = Console.WindowWidth;
        while (!token.IsCancellationRequested)
        {
            try
            {
                // Spara markörens position
                var startLocation = new Point(Console.CursorLeft, Console.CursorTop);

                if ((previousWidth != Console.WindowWidth || Console.CursorTop < configuration.Height) && configuration.AutoAdjustOnResize)
                {
                    ConsoleService.Service.Clear();
                    ConsoleService.Service.WritePrompt();
                    startLocation = new Point((int)IPowerCommandServices.DefaultInstance?.Configuration.Prompt.Length!, configuration.Height);
                }

                // Hämta färgen från konfigurationen
                var backgroundColor = configuration.GetConsoleColor();
                var originalBackgroundColor = Console.BackgroundColor;

                // Sätt bakgrundsfärgen
                Console.BackgroundColor = backgroundColor;

                // Visa datum och klocka på första raden
                Console.SetCursorPosition(0, 0);
                var row1 = $"{DateTime.Now.ToString("dddd d MMMM yyyy HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture)}";
                var row1Short = $"{DateTime.Now.ToString("dddd d MMMM", System.Globalization.CultureInfo.CurrentCulture)}";
                Console.Write(row1.Length > Console.WindowWidth-2 ? row1Short : row1);
                Console.Write(new string(' ', Console.WindowWidth - Console.CursorLeft)); // Rensa resten av raden

                // Visa arbetskatalog på andra raden
                Console.SetCursorPosition(0, 1);
                var row2 = $"dir: {Environment.CurrentDirectory}";
                var row2Short = $"Working directory: {Environment.CurrentDirectory}".GetCompressedPath(Console.WindowWidth-2);
                Console.Write(row2.Length > Console.WindowWidth-2 ? row2Short : row2);
                Console.Write(new string(' ', Console.WindowWidth - Console.CursorLeft)); // Rensa resten av raden

                // Återställ bakgrundsfärgen
                Console.BackgroundColor = originalBackgroundColor;
                
                Console.SetCursorPosition(startLocation.X, startLocation.Y);

                previousWidth = Console.WindowWidth;
                await Task.Delay(configuration.UpdateIntervalSeconds * 1000, token);
            }
            catch (OperationCanceledException)
            {
                break; // Avbrytning förväntad
            }
            catch (Exception ex)
            {
                // Logga oväntade undantag
                Console.WriteLine($"Unhandled exception in RunInfoPanelAsync: {ex.Message}");
            }
        }
    }
    void IDisposable.Dispose()
    {
        _infoPanelTask?.Dispose();
        _cts?.Cancel();
        _cts?.Dispose();
    }
}