namespace Template.Worker.Actions;

public sealed class ActionResult
{
    public static ActionResult None { get; } = new();

    public static ActionResult Invalid { get; } = new();

    public int Succeeded { get; init; }

    public int Skipped { get; init; }

    public int Failed { get; init; }

    public long FreedBytes { get; init; }

    public TimeSpan Elapsed { get; init; }

    public bool Truncated { get; init; }
}
