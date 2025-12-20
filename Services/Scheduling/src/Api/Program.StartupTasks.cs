using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Api;

public interface IStartupTask
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}

[AttributeUsage(AttributeTargets.Class)]
public class OrderAttribute : Attribute
{
    public OrderAttribute(int order)
    {
        Order = order;
    }

    public int Order { get; }
}

public static class StartupTasksExtensions
{
    public static async Task ExecuteStartupTasksAsync(
        this IApplicationBuilder builder)
    {
        var startupTaskTypes = typeof(IStartupTask)
            .Assembly
            .GetTypes()
            .Where(x => !x.IsAbstract && x.IsClass && x.IsAssignableTo(typeof(IStartupTask)))
            .Select(x => new { TheType = x, Order = x.GetCustomAttribute<OrderAttribute>()?.Order ?? 0 })
            .OrderBy(x => x.Order)
            .Select(x => x.TheType);

        foreach (var taskType in startupTaskTypes)
        {
            using var scope = builder.ApplicationServices.CreateScope();

            if (ActivatorUtilities.CreateInstance(scope.ServiceProvider, taskType) is not IStartupTask service)
            {
                continue;
            }

            var logger = scope.ServiceProvider.GetRequiredService<ILogger<IStartupTask>>();

            logger.LogInformation("Starting task {Task}", taskType.Name);

            try
            {
                await service.ExecuteAsync(CancellationToken.None);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error executing task {Task}", taskType.Name);
            }

            logger.LogInformation("Finished task {Task}", taskType.Name);
        }
    }
}
