﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Stashbox.Multitenant;
using System;
using System.Threading.Tasks;

namespace Stashbox.AspNetCore.Multitenant
{
    /// <summary>
    /// Represents a middleware that replaces the current <see cref="IServiceProvider"/> bound to the request with
    /// the desired Stashbox tenant container based on the tenant identifier.
    /// </summary>
    public class StashboxMultitenantMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ITenantDistributor tenantDistributor;

        /// <summary>
        /// Constructs a <see cref="StashboxMultitenantMiddleware"/>.
        /// </summary>
        /// <param name="next">The next item in the request pipeline.</param>
        /// <param name="tenantDistributor">The tenant distributor.</param>
        public StashboxMultitenantMiddleware(RequestDelegate next, ITenantDistributor tenantDistributor)
        {
            this.next = next;
            this.tenantDistributor = tenantDistributor;
        }

        /// <summary>
        /// Replaces the request service provider with the desired tenant container.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="tenantIdExtractor">The tenant id extractor.</param>
        /// <returns>The awaitable task.</returns>
        public async Task InvokeAsync(HttpContext context, ITenantIdExtractor tenantIdExtractor)
        {
            var tenantId = await tenantIdExtractor.GetTenantIdAsync(context).ConfigureAwait(false);
            if(tenantId == null)
            {
                await this.next(context).ConfigureAwait(false);
                return;
            }

            var tenantContainer = this.tenantDistributor.GetTenant(tenantId);
            if (tenantContainer == null)
            {
                await this.next(context).ConfigureAwait(false);
                return;
            }

            IServiceProvidersFeature originalFeature = null;
            try
            {
                originalFeature = context.Features.Get<IServiceProvidersFeature>();
                context.Features.Set<IServiceProvidersFeature>(new RequestServicesFeature(context, 
                    tenantContainer.Resolve<IServiceScopeFactory>()));

                await this.next(context).ConfigureAwait(false);
            }
            finally
            {
                context.Features.Set(originalFeature);
            }
        }
    }
}
