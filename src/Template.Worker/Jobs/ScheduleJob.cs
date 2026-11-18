namespace Template.Worker.Jobs;

using Mofucat.JobScheduler;

using Template.Worker.Actions;
using Template.Worker.Settings;

public sealed class ScheduleJob : ISchedulerJob
{
    private readonly ILogger<ScheduleJob> log;

    private readonly IEnumerable<IAction> actions;

    private readonly WorkerSetting.ScheduleEntry schedule;

    public ScheduleJob(
        ILogger<ScheduleJob> log,
        IEnumerable<IAction> actions,
        WorkerSetting.ScheduleEntry schedule)
    {
        this.log = log;
        this.actions = actions;
        this.schedule = schedule;
    }

    public async ValueTask ExecuteAsync(DateTimeOffset time, CancellationToken cancellationToken)
    {
        var action = actions.FirstOrDefault(x => String.Equals(x.Name, schedule.Action, StringComparison.OrdinalIgnoreCase));
        if (action is null)
        {
            log.WarnActionUnknown(schedule.Action);
            return;
        }

#pragma warning disable CA1031
        try
        {
            log.InfoActionStart(action.Name);
            var result = await action.ExecuteAsync([.. schedule.Arguments], cancellationToken);
            if ((result != ActionResult.None) && (result != ActionResult.Invalid))
            {
                log.InfoActionSummary(action.Name, result.Succeeded, result.Skipped, result.Failed, result.FreedBytes, result.Elapsed, result.Truncated);
            }

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
