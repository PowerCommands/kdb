using PainKiller.PowerCommands.Core.Managers;
using PainKiller.PowerCommands.Shared.Extensions;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Managers;

public class DbManager(string databaseFileName)
{
    public Guid Create(KnowledgeItem item)
    {
        item.ItemID = Guid.NewGuid();
        item.Updated = DateTime.MinValue;
        item.Created = DateTime.Now;
        var db = StorageService<KnowledgeDatabase>.Service.GetObject(databaseFileName);
        db.Items.Add(item);
        Save(db);
        return item.ItemID.Value;
    }
    public int Create(List<KnowledgeItem> items)
    {
        if(items.Count == 0) return 0;
        var db = StorageService<KnowledgeDatabase>.Service.GetObject(databaseFileName);
        var progressbar = new ProgressBar(items.Count);
        foreach (var item in items)
        {
            item.ItemID = Guid.NewGuid();
            item.Updated = DateTime.MinValue;
            item.Created = DateTime.Now;
            db.Items.Add(item);
            progressbar.UpdateOnce();
            progressbar.Show();
        }
        
        Save(db);
        return items.Count;
    }
    public void Delete(Guid itemID)
    {
        var db = StorageService<KnowledgeDatabase>.Service.GetObject(databaseFileName);
        var match = db.Items.First(i => i.ItemID == itemID);
        db.Items.Remove(match);
        Save(db);
    }
    public void Delete(List<KnowledgeItem> items)
    {
        var db = StorageService<KnowledgeDatabase>.Service.GetObject(databaseFileName);
        var progressbar = new ProgressBar(items.Count);
        foreach (var item in items)
        {
            var match = db.Items.First(i => i.ItemID == item.ItemID);
            db.Items.Remove(match);
            progressbar.UpdateOnce();
            progressbar.Show();
        }
        Save(db);
    }
    public IEnumerable<KnowledgeItem> GetAll() => StorageService<KnowledgeDatabase>.Service.GetObject(databaseFileName).Items;

    public IEnumerable<KnowledgeItem> GetItemsWithDuplicateUri()
    {
        var db = StorageService<KnowledgeDatabase>.Service.GetObject(databaseFileName);
        return db.Items
            .GroupBy(item => item.Uri)
            .Where(group => group.Count() > 1)
            .SelectMany(group => group);
    }

    public void RefreshUpdated(KnowledgeItem item)
    {
        var db = StorageService<KnowledgeDatabase>.Service.GetObject(databaseFileName);
        var existing = db.Items.First(i => i.ItemID == item.ItemID);
        db.Items.Remove(existing);
        item.Updated = DateTime.Now;
        db.Items.Add(item);
        Save(db);
    }

    public void Edit(KnowledgeItem item)
    {
        var db = StorageService<KnowledgeDatabase>.Service.GetObject(databaseFileName);
        var existing = db.Items.First(i => i.ItemID == item.ItemID);
        db.Items.Remove(existing);
        item.Updated = DateTime.Now;
        db.Items.Add(item);
        Save(db);
    }
    public KnowledgeDbStats GetStats()
    {
        var db = StorageService<KnowledgeDatabase>.Service.GetObject(databaseFileName);
        var stats = new KnowledgeDbStats{Count = db.Items.Count, DisplayLastUpdated = db.Items.Max(i => i.Updated).GetDisplayTimeSinceLastUpdate(),DisplayFileSize = databaseFileName.GetDisplayFormattedFileSize(),NeverOpenedCount = db.Items.Count(i => i.Updated == DateTime.MinValue)};
        return stats;
    }
    public bool Exists(KnowledgeItem item) => StorageService<KnowledgeDatabase>.Service.GetObject(databaseFileName).Items.Any(i => i.Name == item.Name && i.SourceType == item.SourceType);
    public KnowledgeItem First(Guid itemID) => StorageService<KnowledgeDatabase>.Service.GetObject(databaseFileName).Items.First(i => i.ItemID == itemID);
    public string Backup()
    {
        if (!Directory.Exists(IPowerCommandServices.DefaultInstance!.Configuration.BackupPath)) Directory.CreateDirectory(IPowerCommandServices.DefaultInstance!.Configuration.BackupPath);
        return StorageService<KnowledgeDatabase>.Service.Backup(databaseFileName);
    }
    private void Save(KnowledgeDatabase database) => StorageService<KnowledgeDatabase>.Service.StoreObject(database, databaseFileName);

}