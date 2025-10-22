using BivvySpot.Contracts.Shared;
using BivvySpot.Contracts.v1.Response;

namespace BivvySpot.Presentation.v1.MapToContract;

public static class DifficultyResponseExtensions
{
    public static DifficultyResponse ToContract(this BivvySpot.Model.Entities.Difficulty difficulty)
    {
        return new DifficultyResponse(
            difficulty.Id,
            difficulty.ActivityType.ToContract(),
            difficulty.DifficultyRating
        );
    }

    public static ActivityType ToContract(this BivvySpot.Model.Enums.ActivityType activityType) => activityType switch
    {
        BivvySpot.Model.Enums.ActivityType.Alpine => ActivityType.Alpine,
        BivvySpot.Model.Enums.ActivityType.Ice => ActivityType.Ice,
        BivvySpot.Model.Enums.ActivityType.Mixed => ActivityType.Mixed,
        BivvySpot.Model.Enums.ActivityType.Rock => ActivityType.Rock,
        BivvySpot.Model.Enums.ActivityType.SkiMountaineering => ActivityType.SkiMountaineering,
        BivvySpot.Model.Enums.ActivityType.Hiking => ActivityType.Hiking,
        BivvySpot.Model.Enums.ActivityType.TrailRunning => ActivityType.TrailRunning,
        BivvySpot.Model.Enums.ActivityType.Bouldering => ActivityType.Bouldering,
        BivvySpot.Model.Enums.ActivityType.ViaFerrata => ActivityType.ViaFerrata,
        _ => throw new ArgumentOutOfRangeException(nameof(activityType), $"Not expected ActivityType value: {activityType}"),
    };
}
