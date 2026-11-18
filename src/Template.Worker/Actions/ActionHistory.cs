namespace Template.Worker.Actions;

using System.Text.Json;
using System.Text.Json.Serialization;

using Template.Worker.Settings;

public sealed record ActionHistoryEntry(DateTimeOffset Time, ActionResult Result);

[JsonSourceGenerationOptions(WriteIndented = true, RespectNullableAnnotations = true, RespectRequiredConstructorParameters = true)]
[JsonSerializable(typeof(ActionHistoryEntry))]
internal sealed partial class ActionHistoryJsonContext : JsonSerializerContext;

public sealed class ActionHistory
{
    private readonly ILogger<ActionHistory> log;

    private readonly WorkerSetting setting;

    public ActionHistory(ILogger<ActionHistory> log, WorkerSetting setting)
    {
        this.log = log;
        this.setting = setting;
    }

    public ActionHistoryEntry? Load(string key)
    {
        var path = MakePath(key);
        if (!File.Exists(path))
        {
            return null;
        }

        try
        {
            using var stream = File.OpenRead(path);
            return JsonSerializer.Deserialize(stream, ActionHistoryJsonContext.Default.ActionHistoryEntry);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            log.WarnHistoryReadFailed(ex, path);
            return null;
        }
    }

    public void Save(string key, ActionHistoryEntry entry)
    {
        var path = MakePath(key);
        var temporary = path + ".tmp";
        try
        {
            Directory.CreateDirectory(setting.History.Directory);
            using (var stream = new FileStream(temporary, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                JsonSerializer.Serialize(stream, entry, ActionHistoryJsonContext.Default.ActionHistoryEntry);
                stream.Flush(true);
            }

            File.Move(temporary, path, true);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            log.WarnHistoryWriteFailed(ex, path);
        }
    }

    private string MakePath(string key) => Path.Combine(setting.History.Directory, key + ".json");
}
