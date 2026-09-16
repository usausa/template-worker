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
            await action.ExecuteAsync(arguments.GetParameters(), stoppingToken);
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
