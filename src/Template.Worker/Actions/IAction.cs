namespace Template.Worker.Actions;

public interface IAction
{
    string Name { get; }

    ValueTask<ActionResult> ExecuteAsync(string[] args, CancellationToken cancellationToken);
}
