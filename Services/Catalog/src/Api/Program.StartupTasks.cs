using Api.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

[Order(1)]
public sealed class MigrateDatabaseStartupTask : IStartupTask
{
    readonly CatalogDbContext db;
    readonly bool isCodegen;

    public MigrateDatabaseStartupTask(CatalogDbContext db, IConfiguration configuration)
    {
        this.db = db;
        _ = bool.TryParse(configuration["IS_CODEGEN"], out isCodegen);
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (isCodegen)
        {
            return;
        }

        await db.Database.MigrateAsync(cancellationToken);
    }
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
            var service = ActivatorUtilities.CreateInstance(scope.ServiceProvider, taskType) as IStartupTask;

            if (service == null)
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
