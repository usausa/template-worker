namespace Template.Worker.Actions;

public sealed class ActionArguments
{
    private readonly string[] args;

    public string Name => args[0];

    public ActionArguments(string[] args)
    {
        this.args = args;
    }

    public string[] GetParameters() => args[1..];
}
