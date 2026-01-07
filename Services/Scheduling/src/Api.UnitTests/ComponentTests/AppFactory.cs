using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;
using Xunit;

namespace Api.UnitTests.ComponentTests;

public class AppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private PostgreSqlContainer? postgreSqlContainer;

    private bool disposed;

    async Task IAsyncLifetime.InitializeAsync()
    {
        postgreSqlContainer = new PostgreSqlBuilder().Build();
        await postgreSqlContainer.StartAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await base.DisposeAsync();

        if (postgreSqlContainer != null)
        {
            await postgreSqlContainer.DisposeAsync();
        }
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var connectionString = postgreSqlContainer?.GetConnectionString();
        
        builder.ConfigureHostConfiguration(webBuilder =>
        {
            webBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            { 
                { HostDefaults.EnvironmentKey,    "Development"    },
                { "ConnectionStrings:scheduling", connectionString }
            });
        });

        return base.CreateHost(builder);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposed)
        {
            return;
        }

        disposed = true;

        base.Dispose(disposing);
    }
}

[CollectionDefinition(nameof(AppCollection))]
public class AppCollection : ICollectionFixture<AppFactory>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}

[Collection(nameof(AppCollection))]
public abstract class IntegrationContext : IDisposable
{
    protected readonly AppFactory factory;

    protected IntegrationContext(AppFactory factory)
    {
        this.factory = factory;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
    }
}
