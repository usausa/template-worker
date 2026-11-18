namespace Template.Worker.Workers;

using Template.Worker.Actions;

public sealed class ActionWorker : BackgroundService
{
    private readonly ILogger<ActionWorker> log;

    private readonly IHostApplicationLifetime lifetime;

    private readonly IEnumerable<IAction> actions;

    private readonly ActionArguments arguments;

    public ActionWorker(
        ILogger<ActionWorker> log,
        IHostApplicationLifetime lifetime,
        IEnumerable<IAction> actions,
        ActionArguments arguments)
    {
        this.log = log;
        this.lifetime = lifetime;
        this.actions = actions;
        this.arguments = arguments;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
#pragma warning disable CA1031
        try
        {
            var action = actions.FirstOrDefault(x => String.Equals(x.Name, arguments.Name, StringComparison.OrdinalIgnoreCase));
            if (action is null)
            {
                log.WarnActionUnknown(arguments.Name);
                Environment.ExitCode = -1;
                return;
            }

            log.InfoActionStart(action.Name);
            var result = await action.ExecuteAsync(arguments.GetParameters(), stoppingToken);
            if (result == ActionResult.Invalid)
            {
                Environment.ExitCode = -1;
            }
            else if (result != ActionResult.None)
            {
                log.InfoActionSummary(action.Name, result.Succeeded, result.Skipped, result.Failed, result.FreedBytes, result.Elapsed, result.Truncated);
                if (result.Failed > 0)
                {
                    Environment.ExitCode = 1;
                }
            }

            log.InfoActionEnd(action.Name);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            log.InfoActionCanceled(arguments.Name);
        }
        catch (Exception ex)
        {
            log.ErrorActionFailed(ex, arguments.Name);
            Environment.ExitCode = -1;
        }
        finally
        {
            lifetime.StopApplication();
        }
#pragma warning restore CA1031
    }
}
