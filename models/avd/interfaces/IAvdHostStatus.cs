namespace SmallsOnline.AVD.ResourceManager.Models.AVD;

public interface IAvdHostStatus
{
    string? CurrentStatus { get; set; }
    int? NoSessionsCount { get; set; }
    bool ShutdownOnNextEvaluation { get; set; }

    void IncrementNoSessionsCount();
    void ResetNoSessionsCount();
    bool ShouldShutdown(int threshold);
}