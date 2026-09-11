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

    [LoggerMessage(Level = LogLevel.Information, Message = "Action end. action=[{action}]")]
    public static partial void InfoActionEnd(this ILogger logger, string action);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Unknown action. action=[{action}]")]
    public static partial void WarnActionUnknown(this ILogger logger, string action);

    [LoggerMessage(Level = LogLevel.Error, Message = "Action failed. action=[{action}]")]
    public static partial void ErrorActionFailed(this ILogger logger, Exception ex, string action);

    // Hello

    [LoggerMessage(Level = LogLevel.Information, Message = "Hello. time=[{time}], args=[{args}]")]
    public static partial void InfoHello(this ILogger logger, DateTimeOffset time, string args);

    // Cleanup

    [LoggerMessage(Level = LogLevel.Warning, Message = "Cleanup directory not found. directory=[{directory}]")]
    public static partial void WarnCleanupDirectoryNotFound(this ILogger logger, string directory);

    [LoggerMessage(Level = LogLevel.Information, Message = "Cleanup completed. directory=[{directory}], deleted=[{deleted}]")]
    public static partial void InfoCleanupCompleted(this ILogger logger, string directory, int deleted);
}
