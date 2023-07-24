using System.Text;
using System.Text.Json;

namespace PainKiller.PowerCommands.Core.Services;
public class StorageService<T> : IStorageService<T> where T : new()
{
    private StorageService() { }

    private static readonly Lazy<IStorageService<T>> Lazy = new(() => new StorageService<T>());
    public static IStorageService<T> Service => Lazy.Value;
    public string StoreObject(T storeObject, string fileName = "")
    {
        var fName = string.IsNullOrEmpty(fileName) ? Path.Combine(ConfigurationGlobals.ApplicationDataFolder, $"{typeof(T).Name}.data") : fileName;
        var options = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true };
        var jsonString = JsonSerializer.Serialize(storeObject, options);
        File.WriteAllText(fName, jsonString, Encoding.Unicode);
        return fName;
    }
    public string DeleteObject(string fileName = "")
    {
        var fName = string.IsNullOrEmpty(fileName) ? Path.Combine(ConfigurationGlobals.ApplicationDataFolder, $"{typeof(T).Name}.data") : fileName;
        File.Delete(fName);
        return fName;
    }
    public T GetObject(string fileName = "")
    {
        var fName = string.IsNullOrEmpty(fileName) ? Path.Combine(ConfigurationGlobals.ApplicationDataFolder, $"{typeof(T).Name}.data") : fileName;
        var options = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true };
        if (!File.Exists(fName)) return new T();
        var jsonString = File.ReadAllText(fName);
        return JsonSerializer.Deserialize<T>(jsonString, options) ?? new T();
    }
    public string Backup(string fileName = "")
    {
        var d = DateTime.Now;
        var sourceFilePath = string.IsNullOrEmpty(fileName) ? Path.Combine(ConfigurationGlobals.ApplicationDataFolder, $"{typeof(T).Name}.data") : fileName;
        var backupFilePath = Path.Combine(IPowerCommandServices.DefaultInstance!.Configuration.BackupPath, $"{typeof(T).Name}-{d.Year}{d.Month.ToString().PadLeft(2,'0')}{d.Day.ToString().PadLeft(2,'0')}{d.Hour.ToString().PadLeft(2,'0')}{d.Minute.ToString().PadLeft(2,'0')}{d.Second.ToString().PadLeft(2,'0')}.data");
        var content = File.ReadAllText(sourceFilePath);
        File.WriteAllText(backupFilePath, content);
        return backupFilePath;
    }
}