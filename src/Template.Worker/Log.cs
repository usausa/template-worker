namespace Template.Worker;

internal static partial class Log
{
    // Startup

    [LoggerMessage(Level = LogLevel.Information, Message = "Service start.")]
    public static partial void InfoServiceStart(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Environment: version=[{version}], runtime=[{runtime}], directory=[{directory}]")]
    public static partial void InfoServiceSettingsEnvironment(this ILogger logger, Version? version, Version runtime, string directory);

    [LoggerMessage(Level = LogLevel.Information, Message = "GCSettings: serverGC=[{isServerGC}], latencyMode=[{latencyMode}], largeObjectHeapCompactionMode=[{largeObjectHeapCompactionMode}]")]
    public static partial void InfoServiceSettingsGC(this ILogger logger, bool isServerGC, GCLatencyMode latencyMode, GCLargeObjectHeapCompactionMode largeObjectHeapCompactionMode);

    [LoggerMessage(Level = LogLevel.Information, Message = "ThreadPool: workerThreads=[{workerThreads}], completionPortThreads=[{completionPortThreads}]")]
    public static partial void InfoServiceSettingsThreadPool(this ILogger logger, int workerThreads, int completionPortThreads);

    [LoggerMessage(Level = LogLevel.Information, Message = "Mode: mode=[{mode}]")]
    public static partial void InfoServiceSettingsMode(this ILogger logger, string mode);

    // Action

    [LoggerMessage(Level = LogLevel.Information, Message = "Action start. action=[{action}]")]
    public static partial void InfoActionStart(this ILogger logger, string action);

    [LoggerMessage(Level = LogLevel.Information, Message = "Action summary. action=[{action}], succeeded=[{succeeded}], skipped=[{skipped}], failed=[{failed}], freed=[{freed}], elapsed=[{elapsed}], truncated=[{truncated}]")]
    public static partial void InfoActionSummary(this ILogger logger, string action, int succeeded, int skipped, int failed, long freed, TimeSpan elapsed, bool truncated);

    [LoggerMessage(Level = LogLevel.Information, Message = "Action end. action=[{action}]")]
    public static partial void InfoActionEnd(this ILogger logger, string action);

    [LoggerMessage(Level = LogLevel.Information, Message = "Action canceled. action=[{action}]")]
    public static partial void InfoActionCanceled(this ILogger logger, string action);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Unknown action. action=[{action}]")]
    public static partial void WarnActionUnknown(this ILogger logger, string action);

    [LoggerMessage(Level = LogLevel.Error, Message = "Action failed. action=[{action}]")]
    public static partial void ErrorActionFailed(this ILogger logger, Exception ex, string action);

    // Hello

    [LoggerMessage(Level = LogLevel.Information, Message = "Hello. time=[{time}], args=[{args}]")]
    public static partial void InfoHello(this ILogger logger, DateTimeOffset time, string args);

    // Cleanup

    [LoggerMessage(Level = LogLevel.Warning, Message = "Cleanup policy unknown. policy=[{policy}]")]
    public static partial void WarnCleanupPolicyUnknown(this ILogger logger, string policy);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Cleanup directory not found. policy=[{policy}], directory=[{directory}]")]
    public static partial void WarnCleanupDirectoryNotFound(this ILogger logger, string policy, string directory);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Cleanup delete failed. file=[{file}]")]
    public static partial void WarnCleanupDeleteFailed(this ILogger logger, Exception ex, string file);

    [LoggerMessage(Level = LogLevel.Information, Message = "Cleanup budget exceeded. policy=[{policy}], processed=[{processed}]")]
    public static partial void InfoCleanupBudgetExceeded(this ILogger logger, string policy, int processed);

    [LoggerMessage(Level = LogLevel.Information, Message = "Cleanup summary. policy=[{policy}], succeeded=[{succeeded}], skipped=[{skipped}], failed=[{failed}], freed=[{freed}], elapsed=[{elapsed}], truncated=[{truncated}]")]
    public static partial void InfoCleanupSummary(this ILogger logger, string policy, int succeeded, int skipped, int failed, long freed, TimeSpan elapsed, bool truncated);

    [LoggerMessage(Level = LogLevel.Information, Message = "Cleanup compared. policy=[{policy}], previous=[{previous}], succeeded=[{succeeded:+0;-0;0}], skipped=[{skipped:+0;-0;0}], failed=[{failed:+0;-0;0}], freed=[{freed:+0;-0;0}], elapsed=[{elapsed}]")]
    public static partial void InfoCleanupCompared(this ILogger logger, string policy, DateTimeOffset previous, int succeeded, int skipped, int failed, long freed, TimeSpan elapsed);

    // History

    [LoggerMessage(Level = LogLevel.Warning, Message = "History read failed. file=[{file}]")]
    public static partial void WarnHistoryReadFailed(this ILogger logger, Exception ex, string file);

    [LoggerMessage(Level = LogLevel.Warning, Message = "History write failed. file=[{file}]")]
    public static partial void WarnHistoryWriteFailed(this ILogger logger, Exception ex, string file);
}
