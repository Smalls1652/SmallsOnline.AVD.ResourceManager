using SmallsOnline.AVD.ResourceManager.Models.Azure.DesktopVirtualization;

namespace SmallsOnline.AVD.ResourceManager.Models.AVD;

public class AvdHostStatus : IAvdHostStatus
{
    [JsonConstructor()]
    public AvdHostStatus() {}

    public AvdHostStatus(SessionHost avdSessionHost, AvdHost? previousHostData)
    {
        CurrentStatus = avdSessionHost.Properties?.Status;

        if (previousHostData is null)
        {
            NoSessionsCount = 0;
            ShutdownOnNextEvaluation = false;
        }
    }

    [JsonPropertyName("currentStatus")]
    public string? CurrentStatus { get; set; }

    [JsonPropertyName("noSessionsCount")]
    public int? NoSessionsCount { get; set; }

    [JsonPropertyName("shutdownOnNextEval")]
    public bool ShutdownOnNextEvaluation { get; set; }

    public void IncrementNoSessionsCount()
    {
        NoSessionsCount++;
    }

    public void ResetNoSessionsCount()
    {
        NoSessionsCount = 0;
        ShutdownOnNextEvaluation = false;
    }

    public bool ShouldShutdown(int threshold)
    {
        return NoSessionsCount >= threshold;
    }
}