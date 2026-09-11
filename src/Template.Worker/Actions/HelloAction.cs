namespace Template.Worker.Actions;

public sealed class HelloAction : IAction
{
    private readonly ILogger<HelloAction> log;

    private readonly TimeProvider timeProvider;

    public string Name => "hello";

    public HelloAction(ILogger<HelloAction> log, TimeProvider timeProvider)
    {
        this.log = log;
        this.timeProvider = timeProvider;
    }

    public ValueTask ExecuteAsync(string[] args, CancellationToken cancellationToken)
    {
        log.InfoHello(timeProvider.GetLocalNow(), String.Join(' ', args));
        return ValueTask.CompletedTask;
    }
}
