namespace Template.Worker.Settings;

public sealed class WorkerSetting
{
    [Required]
    public ScheduleEntry Schedule { get; set; } = default!;

    [Required]
    public CleanupEntry Cleanup { get; set; } = default!;

    public sealed class ScheduleEntry
    {
        [Required]
        public string Cron { get; set; } = default!;

        [Required]
        public string Action { get; set; } = default!;

        public List<string> Arguments { get; } = [];
    }

    public sealed class CleanupEntry
    {
        [Required]
        public string Directory { get; set; } = default!;

        [Range(0, 3650)]
        public int RetainDays { get; set; }

        [Range(1, Int32.MaxValue)]
        public int MaxFiles { get; set; }

        [Range(1, 86400)]
        public int MaxSeconds { get; set; }
    }
}
