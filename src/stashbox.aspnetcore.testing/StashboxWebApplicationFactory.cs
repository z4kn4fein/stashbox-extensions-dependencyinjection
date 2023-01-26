﻿using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stashbox.AspNetCore.Multitenant;
using Stashbox.Multitenant;

namespace Stashbox.AspNetCore.Testing;

/// <summary>
/// Stashbox version of <see cref="WebApplicationFactory{TEntryPoint}"/> for bootstrapping an application in memory for functional end to end tests.
/// Test service overrides are organized into Stashbox child containers with an <see cref="ITenantDistributor"/>.
/// </summary>
/// <typeparam name="TEntryPoint">
/// A type in the entry point assembly of the application.
/// Typically the Startup or Program classes can be used.
/// </typeparam>
public class StashboxWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
{
    /// <summary>
    /// The underlying <see cref="ITenantDistributor"/> instance. Handles the different tenants created by <see cref="StashClient"/>.
    /// </summary>
    public ITenantDistributor TenantDistributor { get; }
    
    private int disposed;
    private int disposedAsync;

    /// <summary>
    /// Constructs a <see cref="StashboxWebApplicationFactory{TEntryPoint}"/>.
    /// </summary>
    /// <param name="container">Optional <see cref="IStashboxContainer"/> used as main container for <see cref="TenantDistributor"/>.</param>
    public StashboxWebApplicationFactory(IStashboxContainer? container = null)
    {
        container ??= new StashboxContainer();
        container.Configure(c => c.WithReBuildSingletonsInChildContainer());
        this.TenantDistributor = new TenantDistributor(container);
    }

    /// <summary>
    /// Configures a tenant child container with the given service overrides and produces a <see cref="HttpClient"/>
    /// that makes requests to the configured tenant.
    /// </summary>
    /// <param name="configuration">The <see cref="ServiceCollection"/> and <see cref="WebApplicationFactoryClientOptions"/> configuration options.</param>
    /// <param name="tenantId">Optional tenant identifier used for creating and configuring the underlying tenant. Can be used to access the tenant child container through <see cref="ITenantDistributor.GetTenant"/>.</param>
    /// <returns>The configured <see cref="HttpClient"/>.</returns>
    public HttpClient StashClient(Action<IServiceCollection, WebApplicationFactoryClientOptions> configuration, string? tenantId = null)
    {
        tenantId ??= Guid.NewGuid().ToString();
        var webAppOptions = new WebApplicationFactoryClientOptions();
        var collection = new ServiceCollection();
        configuration(collection, webAppOptions);
        this.TenantDistributor.ConfigureTenant(tenantId, container => container.RegisterServiceDescriptors(collection));
        
        var httpClient = this.CreateClient(webAppOptions);
        httpClient.DefaultRequestHeaders.Add(TenantIdProvider.TenantIdHeader, tenantId);
        return httpClient;
    }

    /// <inheritdoc />
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseStashboxMultitenant<TenantIdProvider>(this.TenantDistributor);
        return base.CreateHost(builder);
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (Interlocked.CompareExchange(ref this.disposed, 1, 0) != 0)
            return;
        
        if (!disposing) return;
        
        this.DisposeAsync().AsTask()
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
        base.Dispose(disposing);
    }

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        if (Interlocked.CompareExchange(ref this.disposedAsync, 1, 0) != 0)
            return;
        
        await this.TenantDistributor.DisposeAsync().ConfigureAwait(false);
        await base.DisposeAsync().ConfigureAwait(false);
        
        GC.SuppressFinalize(this);
    }

    private class TenantIdProvider : ITenantIdExtractor
    {
        public const string TenantIdHeader = "X-TESTING-TENANT-ID";
        
        public Task<object?> GetTenantIdAsync(HttpContext context)
        {
            return Task.FromResult<object?>(!context.Request.Headers.TryGetValue(TenantIdHeader, out var value) 
                ? null 
                : value.First());
        }
    }
}