using BivvySpot.Model.Enums;

namespace BivvySpot.Presentation.v1.MapToModel;

public static class DifficultyExtensions
{
    public static ActivityType ToModel(this Contracts.Shared.ActivityType activityType) => activityType switch
    {
        Contracts.Shared.ActivityType.Alpine => ActivityType.Alpine,
        Contracts.Shared.ActivityType.Ice => ActivityType.Ice,
        Contracts.Shared.ActivityType.Mixed => ActivityType.Mixed,
        Contracts.Shared.ActivityType.Rock => ActivityType.Rock,
        Contracts.Shared.ActivityType.SkiMountaineering => ActivityType.SkiMountaineering,
        Contracts.Shared.ActivityType.Hiking => ActivityType.Hiking,
        Contracts.Shared.ActivityType.TrailRunning => ActivityType.TrailRunning,
        Contracts.Shared.ActivityType.Bouldering => ActivityType.Bouldering,
        Contracts.Shared.ActivityType.ViaFerrata => ActivityType.ViaFerrata,
        _ => throw new ArgumentOutOfRangeException(nameof(activityType), $"Not expected ActivityType value: {activityType}"),
    };
}
