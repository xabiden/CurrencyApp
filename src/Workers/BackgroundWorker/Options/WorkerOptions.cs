using System.ComponentModel.DataAnnotations;

namespace BackgroundWorker.Options;

public sealed class WorkerOptions
{
    public const string SectionName = "Worker";

    [Range(0.01, 8760)]
    public double IntervalHours { get; set; } = 1;
}
