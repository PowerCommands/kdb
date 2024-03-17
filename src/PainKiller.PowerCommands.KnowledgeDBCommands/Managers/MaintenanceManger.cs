using PainKiller.PowerCommands.KnowledgeDBCommands.Enums;
using PainKiller.PowerCommands.KnowledgeDBCommands.Extensions;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Managers;

public class MaintenanceManger(string databaseFileName, IConsoleWriter logWriter)
{
    public void Run()
    {
        var db = StorageService<KnowledgeDatabase>.Service.GetObject(databaseFileName);
        var dbManager = new DbManager(databaseFileName);
        var backupFileName = dbManager.Backup();
        logWriter.WriteSuccess($"File was backed up to {backupFileName}\n\n");

        var deleteItems = new List<KnowledgeItem>();
        foreach (var item in db.Items)
        {
            try
            {
                var sourceType = item.ToItemSourceType();
                if (sourceType == ItemSourceType.OneNote) logWriter.WriteLine($"{ItemSourceType.OneNote} is ignored and will not be affected");
                if (sourceType == ItemSourceType.Url)
                {
                    if(item.Uri.Contains("192.168") || item.Uri.Contains("localhost")) continue;
                    var urlOk = IsUrlOk(item.Uri).Result;
                    if (urlOk)
                    {
                        logWriter.WriteSuccess($"{item.Uri} returnend a valid status code.\n");
                        continue;
                    }

                    logWriter.WriteFailure($"{item.Uri} returnend a bad status code and will be deleted.\n");
                    deleteItems.Add(item);
                }
                if (sourceType == ItemSourceType.Directory || sourceType == ItemSourceType.File)
                {
                    var exists = DoesExist(item.Uri);
                    if (exists)
                    {
                        logWriter.WriteSuccess($"{item.Uri} exists.\n");
                        continue;
                    }

                    logWriter.WriteFailure($"{item.Uri} does not exists and will be deleted.\n");
                    deleteItems.Add(item);
                }
                if(sourceType == ItemSourceType.Unknown)
                {
                    logWriter.WriteLine($"Item has an unknown source type and will be ignored.");
                }
            }
            catch (Exception e)
            {
                logWriter.WriteFailure($"{e.Message}\n");
            }
        }

        logWriter.WriteHeadLine($"\nAll done, number of items to be deleted is: {deleteItems.Count}");
        dbManager.Delete(deleteItems);
    }

    private async Task<bool> IsUrlOk(string url)
    {
        using var httpClient = new HttpClient();
        try
        {
            var response = await httpClient.GetAsync(url);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException e)
        {
            logWriter.WriteFailure($"{e.Message}\n");
            return false;
        }
    }
    private bool DoesExist(string path) => Directory.Exists(path) || File.Exists(path);
}