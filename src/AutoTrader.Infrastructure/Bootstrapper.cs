using Microsoft.Extensions.DependencyInjection;

namespace AutoTrader.Infrastructure;

public static class Bootstrapper
{
   public static IServiceCollection AddInfrastructureRepositories(
       this IServiceCollection services)
   {
       return services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Bootstrapper).Assembly));
   }
}
