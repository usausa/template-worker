namespace Template.Worker.Actions;

using Template.Worker.Settings;

public sealed class CleanupAction : IAction
{
    private readonly ILogger<CleanupAction> log;

    private readonly TimeProvider timeProvider;

    private readonly WorkerSetting setting;

    public string Name => "cleanup";

    public CleanupAction(ILogger<CleanupAction> log, TimeProvider timeProvider, WorkerSetting setting)
    {
        this.log = log;
        this.timeProvider = timeProvider;
        this.setting = setting;
    }

    public ValueTask ExecuteAsync(string[] args, CancellationToken cancellationToken)
    {
        var directory = args.Length > 0 ? args[0] : setting.Cleanup.Directory;
        if (!Directory.Exists(directory))
        {
            log.WarnCleanupDirectoryNotFound(directory);
            return ValueTask.CompletedTask;
        }

        var threshold = timeProvider.GetLocalNow().DateTime.AddDays(-setting.Cleanup.RetainDays);
        var budget = TimeSpan.FromSeconds(setting.Cleanup.MaxSeconds);
        var start = timeProvider.GetTimestamp();
        var deleted = 0;
        var failed = 0;
        foreach (var file in Directory.EnumerateFiles(directory))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if ((deleted + failed >= setting.Cleanup.MaxFiles) || (timeProvider.GetElapsedTime(start) >= budget))
            {
                log.InfoCleanupBudgetExceeded(directory, deleted + failed);
                break;
            }

            if (File.GetLastWriteTime(file) < threshold)
            {
                try
                {
                    File.Delete(file);
                    deleted++;
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    log.WarnCleanupDeleteFailed(ex, file);
                    failed++;
                }
            }
        }

        log.InfoCleanupCompleted(directory, deleted, failed);
        return ValueTask.CompletedTask;
    }
}
