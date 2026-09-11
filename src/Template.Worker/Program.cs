using Microsoft.Extensions.Options;

using Mofucat.JobScheduler.DependencyInjection;

using Serilog;

using Template.Worker;
using Template.Worker.Actions;
using Template.Worker.Jobs;
using Template.Worker.Settings;
using Template.Worker.Workers;

Directory.SetCurrentDirectory(AppContext.BaseDirectory);

var builder = Host.CreateApplicationBuilder(args);

// Service
builder.Services
    .AddWindowsService()
    .AddSystemd();

// Logging
builder.Logging.ClearProviders();
builder.Services.AddSerilog(options =>
{
    options.ReadFrom.Configuration(builder.Configuration);
});

// System
builder.Services.AddSingleton(TimeProvider.System);

// Setting
builder.Services.AddOptions<WorkerSetting>().BindConfiguration("Worker").ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddSingleton(static p => p.GetRequiredService<IOptions<WorkerSetting>>().Value);

// Action
builder.Services.AddSingleton<IAction, HelloAction>();
builder.Services.AddSingleton<IAction, CleanupAction>();

if (args.Length > 0)
{
    // Batch mode
    builder.Services.AddSingleton(new ActionArguments(args));
    builder.Services.AddHostedService<ActionWorker>();
}
else
{
    // Schedule mode
    var setting = builder.Configuration.GetSection("Worker").Get<WorkerSetting>()!;
    builder.Services.AddJobSchedulerService(options =>
    {
        options.UseJob<ScheduleJob>(setting.Schedule.Cron);
    });
}

// Build
var host = builder.Build();

var log = host.Services.GetRequiredService<ILogger<Program>>();

// Startup information
ThreadPool.GetMinThreads(out var workerThreads, out var completionPortThreads);
log.InfoServiceStart();
log.InfoServiceSettingsEnvironment(typeof(Program).Assembly.GetName().Version, Environment.Version, Environment.CurrentDirectory);
log.InfoServiceSettingsGC(GCSettings.IsServerGC, GCSettings.LatencyMode, GCSettings.LargeObjectHeapCompactionMode);
log.InfoServiceSettingsThreadPool(workerThreads, completionPortThreads);
log.InfoServiceSettingsMode(args.Length > 0 ? "batch" : "schedule");

// Run
await host.RunAsync();

return Environment.ExitCode;
