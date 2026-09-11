namespace Template.Worker.Actions;

public interface IAction
{
    string Name { get; }

    ValueTask ExecuteAsync(string[] args, CancellationToken cancellationToken);
}
