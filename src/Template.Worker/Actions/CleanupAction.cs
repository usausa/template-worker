namespace Template.Worker.Actions;

using Template.Worker.Settings;

public sealed class CleanupAction : IAction
{
    private readonly ILogger<CleanupAction> log;

    private readonly TimeProvider timeProvider;

    private readonly WorkerSetting setting;

    private readonly ActionHistory history;

    public string Name => "cleanup";

    public CleanupAction(ILogger<CleanupAction> log, TimeProvider timeProvider, WorkerSetting setting, ActionHistory history)
    {
        this.log = log;
        this.timeProvider = timeProvider;
        this.setting = setting;
        this.history = history;
    }

    public ValueTask<ActionResult> ExecuteAsync(string[] args, CancellationToken cancellationToken)
    {
        var policies = setting.Cleanup.Policies;
        if (args.Length > 0)
        {
            var selected = policies.FirstOrDefault(x => String.Equals(x.Name, args[0], StringComparison.OrdinalIgnoreCase));
            if (selected is null)
            {
                log.WarnCleanupPolicyUnknown(args[0]);
                return ValueTask.FromResult(ActionResult.Invalid);
            }

            policies = [selected];
        }

        var start = timeProvider.GetTimestamp();
        var processed = 0;
        var succeeded = 0;
        var skipped = 0;
        var failed = 0;
        var freed = 0L;
        var truncated = false;
        foreach (var policy in policies)
        {
            var result = Execute(policy, start, ref processed, cancellationToken);
            succeeded += result.Succeeded;
            skipped += result.Skipped;
            failed += result.Failed;
            freed += result.FreedBytes;
            if (result.Truncated)
            {
                truncated = true;
                break;
            }
        }

        return ValueTask.FromResult(new ActionResult
        {
            Succeeded = succeeded,
            Skipped = skipped,
            Failed = failed,
            FreedBytes = freed,
            Elapsed = timeProvider.GetElapsedTime(start),
            Truncated = truncated
        });
    }

    private ActionResult Execute(WorkerSetting.CleanupPolicy policy, long start, ref int processed, CancellationToken cancellationToken)
    {
        var policyStart = timeProvider.GetTimestamp();
        var succeeded = 0;
        var skipped = 0;
        var failed = 0;
        var freed = 0L;
        var truncated = false;

        if (Directory.Exists(policy.Directory))
        {
            var threshold = timeProvider.GetUtcNow().UtcDateTime.AddDays(-policy.RetainDays);
            var budget = TimeSpan.FromSeconds(setting.Cleanup.MaxSeconds);
            var options = new EnumerationOptions
            {
                RecurseSubdirectories = policy.MaxDepth > 0,
                MaxRecursionDepth = policy.MaxDepth,
                IgnoreInaccessible = true,
                AttributesToSkip = FileAttributes.ReparsePoint
            };

            foreach (var file in new DirectoryInfo(policy.Directory).EnumerateFiles("*", options))
            {
                cancellationToken.ThrowIfCancellationRequested();

                if ((processed >= setting.Cleanup.MaxFiles) || (timeProvider.GetElapsedTime(start) >= budget))
                {
                    log.InfoCleanupBudgetExceeded(policy.Name, processed);
                    truncated = true;
                    break;
                }

                if (!IsTarget(policy, file, threshold))
                {
                    skipped++;
                    continue;
                }

                var length = file.Length;
                try
                {
                    file.Delete();
                    succeeded++;
                    freed += length;
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    log.WarnCleanupDeleteFailed(ex, file.FullName);
                    failed++;
                }

                processed++;
            }
        }
        else
        {
            log.WarnCleanupDirectoryNotFound(policy.Name, policy.Directory);
        }

        var result = new ActionResult
        {
            Succeeded = succeeded,
            Skipped = skipped,
            Failed = failed,
            FreedBytes = freed,
            Elapsed = timeProvider.GetElapsedTime(policyStart),
            Truncated = truncated
        };
        log.InfoCleanupSummary(policy.Name, result.Succeeded, result.Skipped, result.Failed, result.FreedBytes, result.Elapsed, result.Truncated);
        Compare(policy, result);
        return result;
    }

    private static bool IsTarget(WorkerSetting.CleanupPolicy policy, FileInfo file, DateTime threshold)
    {
        if (file.LastWriteTimeUtc >= threshold)
        {
            return false;
        }

        if ((policy.Extensions.Count > 0) && !policy.Extensions.Contains(file.Extension, StringComparer.OrdinalIgnoreCase))
        {
            return false;
        }

        return !((file.Length < policy.MinSize) || (file.Length > policy.MaxSize));
    }

    private void Compare(WorkerSetting.CleanupPolicy policy, ActionResult result)
    {
        var key = $"{Name}.{policy.Name}";
        var previous = history.Load(key);
        if (previous is not null)
        {
            log.InfoCleanupCompared(
                policy.Name,
                previous.Time,
                result.Succeeded - previous.Result.Succeeded,
                result.Skipped - previous.Result.Skipped,
                result.Failed - previous.Result.Failed,
                result.FreedBytes - previous.Result.FreedBytes,
                result.Elapsed - previous.Result.Elapsed);
        }

        history.Save(key, new ActionHistoryEntry(timeProvider.GetLocalNow(), result));
    }
}
