namespace Template.Worker.Jobs;

using Mofucat.JobScheduler;

using Template.Worker.Actions;
using Template.Worker.Settings;

public sealed class ScheduleJob : ISchedulerJob
{
    private readonly ILogger<ScheduleJob> log;

    private readonly WorkerSetting setting;

    private readonly IEnumerable<IAction> actions;

    public ScheduleJob(
        ILogger<ScheduleJob> log,
        WorkerSetting setting,
        IEnumerable<IAction> actions)
    {
        this.log = log;
        this.setting = setting;
        this.actions = actions;
    }

    public async ValueTask ExecuteAsync(DateTimeOffset time, CancellationToken cancellationToken)
    {
        var action = actions.FirstOrDefault(x => String.Equals(x.Name, setting.Schedule.Action, StringComparison.OrdinalIgnoreCase));
        if (action is null)
        {
            log.WarnActionUnknown(setting.Schedule.Action);
            return;
        }

#pragma warning disable CA1031
        try
        {
            log.InfoActionStart(action.Name);
            await action.ExecuteAsync([.. setting.Schedule.Arguments], cancellationToken);
            log.InfoActionEnd(action.Name);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            log.InfoActionCanceled(action.Name);
        }
        catch (Exception ex)
        {
            log.ErrorActionFailed(ex, action.Name);
        }
#pragma warning restore CA1031
    }
}
