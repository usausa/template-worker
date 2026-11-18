namespace Template.Worker.Settings;

using Microsoft.Extensions.Options;

using Mofucat.JobScheduler;

public sealed class WorkerSetting : IValidatableObject
{
    [ValidateEnumeratedItems]
    public List<ScheduleEntry> Schedules { get; } = [];

    [Required]
    [ValidateObjectMembers]
    public CleanupEntry Cleanup { get; set; } = default!;

    [Required]
    [ValidateObjectMembers]
    public HistoryEntry History { get; set; } = default!;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var schedule in Schedules)
        {
            if (!String.IsNullOrEmpty(schedule.Name) && !names.Add(schedule.Name))
            {
                yield return new ValidationResult($"Schedule name is duplicated. name=[{schedule.Name}]", [nameof(Schedules)]);
            }
        }
    }

    public sealed class ScheduleEntry
    {
        [Required]
        public string Name { get; set; } = default!;

        [Required]
        public string Cron { get; set; } = default!;

        [Required]
        public string Action { get; set; } = default!;

        public List<string> Arguments { get; } = [];

        public MisfirePolicy Misfire { get; set; } = MisfirePolicy.Skip;

        [Range(0, Int32.MaxValue)]
        public int MaxCatchUp { get; set; }
    }

    public sealed class CleanupEntry : IValidatableObject
    {
        [Range(1, Int32.MaxValue)]
        public int MaxFiles { get; set; }

        [Range(1, 86400)]
        public int MaxSeconds { get; set; }

        [ValidateEnumeratedItems]
        public List<CleanupPolicy> Policies { get; } = [];

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var policy in Policies)
            {
                if (!String.IsNullOrEmpty(policy.Name) && !names.Add(policy.Name))
                {
                    yield return new ValidationResult($"Cleanup policy name is duplicated. name=[{policy.Name}]", [nameof(Policies)]);
                }
            }
        }
    }

    public sealed class CleanupPolicy : IValidatableObject
    {
        [Required]
        public string Name { get; set; } = default!;

        [Required]
        public string Directory { get; set; } = default!;

        [Range(0, 3650)]
        public int RetainDays { get; set; }

        public List<string> Extensions { get; } = [];

        [Range(0, 32)]
        public int MaxDepth { get; set; }

        [Range(0, Int64.MaxValue)]
        public long? MinSize { get; set; }

        [Range(0, Int64.MaxValue)]
        public long? MaxSize { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                yield return new ValidationResult($"Cleanup policy name contains invalid characters. name=[{Name}]", [nameof(Name)]);
            }

            if (MinSize > MaxSize)
            {
                yield return new ValidationResult($"Cleanup policy size range is invalid. name=[{Name}]", [nameof(MinSize), nameof(MaxSize)]);
            }
        }
    }

    public sealed class HistoryEntry
    {
        [Required]
        public string Directory { get; set; } = default!;
    }
}
