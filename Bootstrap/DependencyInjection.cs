using Incremental.Application.Interfaces;
using Incremental.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Incremental.Game.Bootstrap;

public static class DependencyInjection
{
    public static ServiceProvider Build()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IBoatMovementService, BoatMovementService>();
        services.AddSingleton<IFishSpawnService, FishSpawnService>();
        services.AddSingleton<IFishDetectionService, FishDetectionService>();
        services.AddSingleton<IFishCaptureService, FishCaptureService>();

        return services.BuildServiceProvider();
    }
}
