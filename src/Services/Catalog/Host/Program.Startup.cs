using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Api.Host.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Api.Host;

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

public abstract class StartupTaskBase : IStartupTask
{
    readonly bool isCodegen;
    
    protected StartupTaskBase(IConfiguration configuration)
    {
        _ = bool.TryParse(configuration["IS_CODEGEN"], out isCodegen);
    }

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return ExecuteImplAsync(isCodegen, cancellationToken);
    }

    protected abstract Task ExecuteImplAsync(
        bool isCodegen,
        CancellationToken cancellationToken);
}

[Order(1)]
public sealed class MigrateDatabaseStartupTask : StartupTaskBase
{
    readonly AudioCatalogDbContext db;

    public MigrateDatabaseStartupTask(
        AudioCatalogDbContext db,
        IConfiguration configuration) : base(configuration)
    {
        this.db = db;
    }

    protected override async Task ExecuteImplAsync(
        bool isCodegen,
        CancellationToken cancellationToken)
    {
        if (isCodegen)
        {
            return;
        }

        await db.Database.MigrateAsync(cancellationToken);
    }
}

[Order(2)]
public sealed class SeedCategoriesTask : StartupTaskBase
{
    private static readonly Category[] FixedCategories = new[]
    {
        new Category { Path = "/", Name = "(root)" },
        new Category { Path = "/music", Name = "Music", Parent = "/" },
        new Category { Path = "/idents", Name = "Idents", Parent = "/" },
        new Category { Path = "/beds", Name = "Beds", Parent = "/" },
        new Category { Path = "/trails", Name = "Trails", Parent = "/" }
    };
    
    readonly AudioCatalogDbContext db;
    
    public SeedCategoriesTask(
        AudioCatalogDbContext db,
        IConfiguration configuration) : base(configuration)
    {
        this.db = db;
    }

    protected override async Task ExecuteImplAsync(
        bool isCodegen,
        CancellationToken cancellationToken)
    {
        if (isCodegen)
        {
            return;
        }

        var list = await db.Categories
            .AsNoTracking()
            .ToArrayAsync(cancellationToken);

        foreach (var category in FixedCategories)
        {
            if (!list.Any(x => x.Path == category.Path))
            {
                db.Categories.Add(category);
            }
        }

        await db.SaveChangesAsync(cancellationToken);
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
